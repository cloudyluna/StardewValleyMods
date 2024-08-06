# SelectiveEating by Cloudyluna

A simple quality of life mod to make eating easier and automatic with
configurable settings.

### Features

- Automatically eats food based on health or stamina percentage
  deprivation.

- Forbid certain food from being eaten.

- Prioritize and make a list of food as the most important ones to be
  consumed first.

- An option (default) to stay in the same facing direction even after
  eating.

- Increase or decrease eating time interval for roleplaying/challenge or
  performance reasons.

- Options can be configured through [Generic Config Menu
  mod](https://www.nexusmods.com/stardewvalley/mods/5098) or
  `config.json` file within the mod directory.

## Building

> Attention: Make sure you already have set up $*GAME_PATH* pointing to
> your Stardew Valley game folder first.

If you cloned this project from the Github and have `nix` installed, run
`nix develop` to load the development environment. Then run `make` or
`dotnet build` to build the project.

            # Example to build SelectiveEating for release
            git clone https://github.com/cloudyluna/StardewValleyMods
            cd SelectiveEating
            make release

## URL link to the repository

[Github
repository](https://github.com/cloudyluna/StardewValleyMods/tree/main/SelectiveEating)

## Thanks to

- [SMAPI dev and contributors](https://github.com/Pathoschild/SMAPI) for
  making Stardew Valley modding accessible.

- [Generic Config Menu dev and
  contributors](https://www.nexusmods.com/stardewvalley/mods/5098) for
  making mod configuration through GUI, simple and easy.

## License

- This project is licensed under the AGPL-3.0-or-later. See the
  `LICENSE` file for details.

- The bundled FSharp.Core is licensed under the MIT license. See
  `sublicenses/FSharp.MIT` file for details.
