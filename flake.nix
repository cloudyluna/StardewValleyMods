{
  description = ''
    '';

  inputs = {
    nixpkgs.url      = "github:NixOS/nixpkgs/nixos-unstable";
    flake-utils.url  = "github:numtide/flake-utils";
  };
  
  outputs = { self, nixpkgs, flake-utils, ... }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        overlays = [];
        pkgs = import nixpkgs {
          inherit system;
          allowUnfree = true;
        };
      in
      {
        devShells.default = with pkgs; let
          dotnet6 = pkgs.dotnetCorePackages.sdk_6_0_1xx.overrideAttrs (oldAttrs: {
              postInstall = oldAttrs.postInstall + "ln -s $out/dotnet $out/bin/dotnet6";
          });
           in mkShell rec {
          buildInputs = [
            dotnetCorePackages.sdk_8_0_3xx
            dotnet6
            ilspycmd # decompile SDV because there are so many things that
                     # aren't simply present in public documentation.
            avalonia-ilspy
            zip
            unzip
            gnumake
            pandoc

            # vscode in FHS environment
            vscode-fhs
            vscode-extensions.ionide.ionide-fsharp

            # art
            krita
            tiled
            
          ];

          LD_LIBRARY_PATH = "${lib.makeLibraryPath buildInputs}";

          shellHook = ''
            export STARDEW_VALLEY="$HOME/.sdv_dev/drive_c/GOG Games/Stardew Valley/"
          '';
        };
      }
    );
}
