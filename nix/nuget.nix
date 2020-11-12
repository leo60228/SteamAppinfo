{ fetchurl, unzip, linkFarm, lib, stdenvNoCC }:
let fetchNuGet = { baseName, version, sha512 }:
  let nupkgName = lib.strings.toLower "${baseName}.${version}.nupkg"; in
  stdenvNoCC.mkDerivation {
    name = "${baseName}-${version}";
    
    src = fetchurl {
      inherit sha512;
      url = "https://www.nuget.org/api/v2/package/${baseName}/${version}";
      name = "${baseName}.${version}.zip";
    };
    
    sourceRoot = ".";
    
    buildInputs = [ unzip ];
    
    dontStrip = true;
    
    installPhase = ''
      mkdir -p $out
      chmod +r *.nuspec
      cp *.nuspec $out
      cp $src $out/${nupkgName}
    '';
  };
in
name: rec {
  cache = linkFarm "${name}-nuget-pkgs" packages;
  packages = [
    { name = "Microsoft.AspNetCore.App.Runtime.linux-x64";
      path = fetchNuGet {
        baseName = "Microsoft.AspNetCore.App.Runtime.linux-x64";
        version = "3.1.2";
        sha512 = "2ydlc0bihlg89dr7h6hsvragrhxk0k1w30ybqyhv9xnswkdrb20snvgx2k380cwh8aiymxvpjcpbw5xlp24zikdxci32ij5dkn03id4";
      };
    }
    { name = "Microsoft.NETCore.App.Host.linux-x64";
      path = fetchNuGet {
        baseName = "Microsoft.NETCore.App.Host.linux-x64";
        version = "3.1.2";
        sha512 = "03p5mkix8mbxmj1ay9aifj2vhxsfmg3wqi9vgpwdv9pnrwv4fg6cw1icy0kiwjcv9s8i4nqz9nf442mms2g73jwp5mcfd0i82rlzwaq";
      };
    }
    { name = "Microsoft.NETCore.App.Runtime.linux-x64";
      path = fetchNuGet {
        baseName = "Microsoft.NETCore.App.Runtime.linux-x64";
        version = "3.1.2";
        sha512 = "164dy9zpal1bc6zi4xmfx6yq7x2gzi0qspdy1j34xybk2m5radmw5hq9rzqar77r48xrmywc631ghfqx0wxlb35rmj86p3xf4yd2iip";
      };
    }
    { name = "AspNetCoreRateLimit";
      path = fetchNuGet {
        baseName = "AspNetCoreRateLimit";
        version = "3.2.2";
        sha512 = "2xzzz4c5rkhvv0g7inkrlgb3lcg7ggcrrl9fz51raf5prim3ir8skf9nfs9q1rqz1qaifiy65sdwlj3drxnjv8jd9xwqakqhggid1n9";
      };
    }
    { name = "LettuceEncrypt";
      path = fetchNuGet {
        baseName = "LettuceEncrypt";
        version = "1.0.0";
        sha512 = "07n4b31y97vf7plgjfrqxk0djli8ag7sgdawf3nm9wazwdmcz5phvqdvxk1h9yph01cfc74smidi6z4n1wxzjlqdymqpl5h6hy8f4ac";
      };
    }
    { name = "SteamKit2";
      path = fetchNuGet {
        baseName = "SteamKit2";
        version = "2.3.0";
        sha512 = "2wkrigd4j8r9l71f9mvjwjgilnnjr7l06z1ksw9gam4h4c21n5b5y017w3xig8p0blm54q3mng664bxp4rinnni5vcjb5pnh17m0w2v";
      };
    }
    { name = "Certes";
      path = fetchNuGet {
        baseName = "Certes";
        version = "2.3.3";
        sha512 = "3hydlykgjwi2pqq9kdnbllv0l1zwm40vvx6w4albbwvvnxzcsfmg8pp6ik24ikkvzzz3vbj8lm5177rw39jqwsj4lkhwzv6ymm6v9bb";
      };
    }
    { name = "Microsoft.Extensions.Caching.Abstractions";
      path = fetchNuGet {
        baseName = "Microsoft.Extensions.Caching.Abstractions";
        version = "3.1.9";
        sha512 = "2g6wrx0xvyr3rj94g3iy92zgf1jnkjkbz7bi1r16aqbnqb7dhjbj1pqr21rhrhqs537pzmp50zxqknj4pf9knxkfwz6bgydhzs7jfjn";
      };
    }
    { name = "Microsoft.Extensions.DependencyInjection.Abstractions";
      path = fetchNuGet {
        baseName = "Microsoft.Extensions.DependencyInjection.Abstractions";
        version = "3.1.9";
        sha512 = "2hxnd988q23xay9za6hsdac6y4gdnxyshgyfrphkv8shciwwfilpzakcb6a4zvlv459bzny3sqz7iljwzn0wiakvjp0v3aa39hb1f59";
      };
    }
    { name = "Microsoft.Extensions.Logging.Abstractions";
      path = fetchNuGet {
        baseName = "Microsoft.Extensions.Logging.Abstractions";
        version = "3.1.9";
        sha512 = "10c7hpq9wwgklzpymjrcf6hfkjzs018cg7pw6bs4dv325a0xg4gmh3r9vvknrs2s4irr2qwarill8205sdswsmsnjbd81vvy1x2q182";
      };
    }
    { name = "Microsoft.Extensions.Options";
      path = fetchNuGet {
        baseName = "Microsoft.Extensions.Options";
        version = "3.1.9";
        sha512 = "1vr2lci01sqk657j61n5fms0wf1nq6h3mybm871ahxwrx80rw8k6pl8386jvxr9srfbhk5afj7njywqm8xyy8grywnhnkns1c10cj0n";
      };
    }
    { name = "Microsoft.Extensions.Primitives";
      path = fetchNuGet {
        baseName = "Microsoft.Extensions.Primitives";
        version = "3.1.9";
        sha512 = "253dr90a7w2p30jgpfqg1ls08wg9gmcx8vw0g0vliml58hl2fwqsy4g2nkbih7wrj8pz2i5k8mzpwlb824h4fb7mnyj451n6i4ggrz9";
      };
    }
    { name = "Microsoft.NETCore.Platforms";
      path = fetchNuGet {
        baseName = "Microsoft.NETCore.Platforms";
        version = "3.1.0";
        sha512 = "1kdj31pshvpv3agzx97z9b8097q5r4v8ws644xzffxfpph72iijvnv2rrrb21g8xp1vwnrrasvva8n12qbcd3mijg8wx0ppd0viwsk3";
      };
    }
    { name = "Microsoft.Win32.Registry";
      path = fetchNuGet {
        baseName = "Microsoft.Win32.Registry";
        version = "4.7.0";
        sha512 = "01nxm5g8qsidaiaxv45zg4xbrr1kfgrx6dmany3zdl0v7dn6hsqhag82v5jpzf09y54191klcx988hp4d6vhgb0granbmkb93xxz8zx";
      };
    }
    { name = "Newtonsoft.Json";
      path = fetchNuGet {
        baseName = "Newtonsoft.Json";
        version = "12.0.3";
        sha512 = "3skbix5pwilpagjs2mvm7z5g1hm0lymbcafc7p0pkm282y219k0xzsq2fyb036lq45myycj9lpsfdfl2szz4i3ck6z8pibr0igncd39";
      };
    }
    { name = "Portable.BouncyCastle";
      path = fetchNuGet {
        baseName = "Portable.BouncyCastle";
        version = "1.8.1.4";
        sha512 = "10rls702436swf3jaz79shccyb6cdh0wbfkqrkr69lgfw5m6nyb1q3d7k8x5z8g5my6zw7lwgvxzp20sz5xb51waw4w0jg532gcdl4n";
      };
    }
    { name = "protobuf-net";
      path = fetchNuGet {
        baseName = "protobuf-net";
        version = "2.4.6";
        sha512 = "2cd9f7ma985z2mjvpxc42lzci9179v0cgkip99rjcc05ivlzcm09pf62awbd6srvywfz0bqq0rhp3wqq3w0lwxw9xj5yi5bnf85mc3g";
      };
    }
    { name = "System.Private.ServiceModel";
      path = fetchNuGet {
        baseName = "System.Private.ServiceModel";
        version = "4.5.3";
        sha512 = "36wd65kpmapl0mpd5gscfhadr7nfady22avc0czxs47l0xv0awxjbsrdphrj30wyl317mqy2djyip4qcrc6l1advgm1zgbr5cx3wlqf";
      };
    }
    { name = "System.Reflection.DispatchProxy";
      path = fetchNuGet {
        baseName = "System.Reflection.DispatchProxy";
        version = "4.5.0";
        sha512 = "3nr5l0mgdgchjc0mp33izmjxl27w71gvi2az2xcdxv114c130q3g1v88n2112lraa9y3v9bndsk0mbn9b6pvr6gjc1156mj6myj55nk";
      };
    }
    { name = "System.Security.AccessControl";
      path = fetchNuGet {
        baseName = "System.Security.AccessControl";
        version = "4.7.0";
        sha512 = "2pbbshdlxm27glwnac8qxv90f7s6x23cr2yfkl90pp031kp1k82g9ld3z15wrl0n6nwb89pclcz8pf74slixalyy06rmbf13j45ahj6";
      };
    }
    { name = "System.Security.Principal.Windows";
      path = fetchNuGet {
        baseName = "System.Security.Principal.Windows";
        version = "4.7.0";
        sha512 = "2bawk3ddnsknjrb1gqdxbw0ip80ai9f93n77kxlbsdlvv6bs4xvzc2zpwfzgaqrrw2i8i67qa5v5jh0nal660r4n9hdp4l79k9ic2pk";
      };
    }
    { name = "System.ServiceModel.Primitives";
      path = fetchNuGet {
        baseName = "System.ServiceModel.Primitives";
        version = "4.5.3";
        sha512 = "076gq1xly01k3vjhk40hv6dgnnm0frdykcc9fnp9a8c5f5x7rfff7lia88vf1ayqzzy6sw6r3x0g5cl0d0q1f06br2x1rsy1414z1d5";
      };
    }
  ];
}
