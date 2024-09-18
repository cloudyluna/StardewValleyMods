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
        };
      in
      {
        devShells.default = with pkgs; mkShell rec {
          buildInputs = [
            dotnetCorePackages.sdk_8_0_3xx
            ilspycmd # decompile SDV because there are so many things that
                     # aren't simply present in public documentation.
            jq
            moreutils
            zip
            unzip
            gnumake
            pandoc
            texliveBasic
            git
            
          ];

          LD_LIBRARY_PATH = "${lib.makeLibraryPath buildInputs}";

          shellHook = ''
            export STARDEW_VALLEY="$HOME/.sdv_dev/drive_c/GOG Games/Stardew Valley"
          '';
        };
      }
    );
}
