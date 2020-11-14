using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AspNetCoreRateLimit;
using LettuceEncrypt;

namespace SteamAppinfo {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddOptions();
            services.AddMemoryCache();

            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddSingleton<IGetProductInfo, SteamConnection>();

            services.AddMvc().AddJsonOptions(options => {
                options.JsonSerializerOptions.Converters.Add(new KeyValueConverter());
                options.JsonSerializerOptions.Converters.Add(new ByteArrayConverter());
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            var lettuce = services.AddLettuceEncrypt();
            string persist = Configuration["LettuceEncrypt:PersistPath"];
            if (persist != null) {
                string persistPass = Configuration["LettuceEncrypt:PersistPass"];
                lettuce.PersistDataToDirectory(new DirectoryInfo(persist), persistPass);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseExceptionHandler("/error");

            app.UseIpRateLimiting();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
