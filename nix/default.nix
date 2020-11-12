{ stdenv, libunwind, openssl, icu, libuuid, zlib, curl, callPackage, dotnet-sdk_3 }:
let
  rpath = stdenv.lib.makeLibraryPath [ stdenv.cc.cc libunwind libuuid icu openssl zlib curl ];
  nugetPkgs = callPackage (import ./nuget.nix) {} "SteamAppinfo";
in stdenv.mkDerivation rec {
  pname = "SteamAppinfo";
  version = "2020-11-12";

  src = ./..;

  buildInputs = [ dotnet-sdk_3 nugetPkgs.cache ];

  buildPhase = ''
    export DOTNET_NOLOGO=1
    export DOTNET_CLI_TELEMETRY_OPTOUT=1
    export HOME="$(mktemp -d)"
    dotnet publish --nologo \
      -r linux-x64 --self-contained \
      --source "${nugetPkgs.cache}" -c Release -o out
  '';

  installPhase = ''
    runHook preInstall
    mkdir -p $out/bin
    cp -r ./out/* $out
    ln -s $out/SteamAppinfo $out/bin/SteamAppinfo
    runHook postInstall
  '';

  dontPatchELF = true;
  postFixup = ''
    patchelf \
      --set-interpreter "${stdenv.cc.bintools.dynamicLinker}" \
      --set-rpath '$ORIGIN:${rpath}' $out/SteamAppinfo
    find $out -type f -name "*.so" -exec \
      patchelf --set-rpath '$ORIGIN:${rpath}' {} ';'
  '';
}
