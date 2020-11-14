using System;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Net;
using SteamKit2;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SteamAppinfo {
    public class SteamConnection : IGetProductInfo, IDisposable {
        public ILogger Logger { get; }
        public IConfiguration Configuration { get; }

        SteamClient steamClient;
        CallbackManager manager;

        string user;
        string pass;
        string guardCode;

        SteamUser steamUser;
        SteamApps steamApps;

        volatile bool loggedIn;
        volatile bool disconnected = false;

        public SteamConnection(ILogger<SteamConnection> logger, IConfiguration configuration) {
            Logger = logger;
            Configuration = configuration;

            user = Configuration["Steam:Username"];
            pass = Configuration["Steam:Password"];
            guardCode = Configuration["Steam:GuardCode"];
        }

        async Task connect() {
            try {
                Logger.LogInformation("Initializing client...");

                steamClient = new SteamClient();
                manager = new CallbackManager(steamClient);

                steamUser = steamClient.GetHandler<SteamUser>();

                steamApps = steamClient.GetHandler<SteamApps>();

                manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
                manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
                manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
                manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
                manager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth);
                manager.Subscribe<SteamUser.LoginKeyCallback>(OnLoginKey);

                Task logIn = Task.Factory.StartNew(
                    () => {
                        Logger.LogInformation("Connecting to Steam...");
                        steamClient.Connect();
                        while (!loggedIn) {
                            manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
                        }
                    },
                    new CancellationToken(false),
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default
                );

                await logIn;
            } catch {
                disconnected = true;

                if (steamUser != null) {
                    steamUser.LogOff();
                }

                if (steamClient.IsConnected) {
                    steamClient.Disconnect();
                }

                throw;
            }
        }

        void OnConnected(SteamClient.ConnectedCallback callback) {
            if (user != null) {
                Logger.LogInformation("Connected to Steam! Logging in '{0}'...", user);

                byte[] sentryHash = null;
                if (File.Exists("sentry.bin")) {
                    Logger.LogInformation("Loading saved sentry.bin...");
                    byte[] sentryFile = File.ReadAllBytes("sentry.bin");
                    sentryHash = CryptoHelper.SHAHash(sentryFile);
                }

                string loginKey = null;
                if (File.Exists("loginkey.bin")) {
                    Logger.LogInformation("Loading saved login key...");
                    loginKey = File.ReadAllText("loginkey.bin");
                }

                steamUser.LogOn(new SteamUser.LogOnDetails {
                    Username = user,
                    Password = pass,
                    AuthCode = guardCode,
                    SentryFileHash = sentryHash,
                    ShouldRememberPassword = true,
                    LoginKey = loginKey,
                });
            } else {
                Logger.LogInformation("Connected to Steam! Logging in anonymously...");
                steamUser.LogOnAnonymous();
            }
        }

        void OnDisconnected(SteamClient.DisconnectedCallback callback) {
            Logger.LogInformation("Disconnected.");
            if (!disconnected) {
                Logger.LogInformation("Unexpected disconnection, preparing relog...");
                loggedIn = false;
            }
        }

        void OnLoggedOn(SteamUser.LoggedOnCallback callback) {
            if (callback.Result != EResult.OK) {
                throw new WebException($"Unable to logon to Steam: {callback.Result} / {callback.ExtendedResult}");
            }

            Logger.LogInformation("Waiting five seconds to receive extra events...");
            Task.Delay(5000).ContinueWith(x => {
                Logger.LogInformation("Done!");
                loggedIn = true;
            });
        }

        void OnLoggedOff(SteamUser.LoggedOffCallback callback) {
            Logger.LogInformation("Logged off.");
            if (!disconnected) {
                Logger.LogInformation("Unexpected logoff, preparing relog...");
                loggedIn = false;
                if (steamClient.IsConnected) {
                    Logger.LogInformation("Still connected, disconnecting...");
                    steamClient.Disconnect();
                }
            }
        }

        public async Task<SteamApps.PICSProductInfoCallback.PICSProductInfo> ProductInfo(uint app) {
            if (manager != null) {
                await Task.Run(() => {
                    manager.RunWaitAllCallbacks(TimeSpan.Zero);
                });
            }

            if (!loggedIn) {
                await connect();
            }

            Logger.LogInformation("Getting product info...");
            var productJob = steamApps.PICSGetProductInfo(app, package: null);
            var resultSet = await productJob;

            if (resultSet.Complete) {
                var productInfoCallback = resultSet.Results.First();
                var productInfo = productInfoCallback.Apps.Values.FirstOrDefault();
                if (productInfo != null) {
                    Logger.LogInformation("Got product info!");
                    return productInfo;
                } else {
                    throw new WebException("No product info!");
                }
            } else if (resultSet.Failed) {
                var productInfoCallback = resultSet.Results.FirstOrDefault(prodCallback => prodCallback.Apps.ContainsKey(app));

                if (productInfoCallback != null) {
                    var productInfo = productInfoCallback.Apps.Values.First();
                    Logger.LogWarning("Received product info with server-side error");
                    return productInfo;
                } else {
                    throw new WebException("Server side error!");
                }
            } else {
                var productInfoCallback = resultSet.Results.FirstOrDefault(prodCallback => prodCallback.Apps.ContainsKey(app));

                if (productInfoCallback != null) {
                    var productInfo = productInfoCallback.Apps.Values.First();
                    Logger.LogWarning("Received product info with timeout");
                    return productInfo;
                } else {
                    throw new WebException(new TimeoutException("Timed out!"));
                }
            }
        }

        void OnMachineAuth(SteamUser.UpdateMachineAuthCallback callback) {
            Logger.LogInformation("Writing sentry.bin...");
            int fileSize;
            byte[] sentryHash;
            using (var fs = File.Open("sentry.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                fs.Seek(callback.Offset, SeekOrigin.Begin);
                fs.Write(callback.Data, 0, callback.BytesToWrite);
                fileSize = (int)fs.Length;
                fs.Seek(0, SeekOrigin.Begin);

                using var sha = SHA1.Create();
                sentryHash = sha.ComputeHash(fs);
            }

            Logger.LogInformation("Accepting sentry...");
            steamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails {
                JobID = callback.JobID,
                FileName = callback.FileName,
                BytesWritten = callback.BytesToWrite,
                FileSize = fileSize,
                Offset = callback.Offset,
                Result = EResult.OK,
                LastError = 0,
                OneTimePassword = callback.OneTimePassword,
                SentryFileHash = sentryHash,
            });
            Logger.LogInformation("Accepted!");
        }

        void OnLoginKey(SteamUser.LoginKeyCallback callback) {
            Logger.LogInformation("Writing login key...");
            File.WriteAllText("loginkey.bin", callback.LoginKey);
            Logger.LogInformation("Accepting...");
            steamUser.AcceptNewLoginKey(callback);
            Logger.LogInformation("Done!");
        }

        public void Dispose() {
            Logger.LogInformation("Disconnecting...");

            disconnected = true;

            if (steamUser != null) {
                steamUser.LogOff();
            }

            if (steamClient.IsConnected) {
                steamClient.Disconnect();
            }
        }
    }
}
