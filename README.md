# StardewValleyMods by Cloudyluna

My personal collection of Stardew Valley mods that I wrote (and ongoing)
for fun. Nothing serious here.

## Building

If you have `nix` installed, run `nix develop` to load the development
environment. In a nutshell, `cd` into any *subproject* and run `make` to
build the project.

> Attention: Make sure you have setup \$*GAME_PATH* pointing to your
> Stardew Valley game folder first.

            # Example to build SelectiveEating
            git clone https://github.com/cloudyluna/StardewValleyMods
            cd SelectiveEating
            make

## Thanks to

- [SMAPI dev and contributors](https://github.com/Pathoschild/SMAPI) for
  making Stardew Valley modding accessible.

- [Generic Config Menu dev and
  contributors](https://www.nexusmods.com/stardewvalley/mods/5098) for
  making mod configuration through GUI, simple and easy.

## License

This project is licensed under the AGPL-3.0-or-later License - see the
`LICENSE` file for details.
