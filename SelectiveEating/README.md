# Selective Eating

------------------------------------------------------------------------

A simple quality-of-life mod to make eating easier and automatic with
configurable settings.

## Features

- Automatically eat food when you lose some health or energy. You can
  configure this eating priority strategy in the menu to either
  prioritize replenishing the most health or energy as much as possible,
  the lowest priced food to save money, or turn this extra strategy off
  at any time.

- Entirely skip eating animation (and the world pauses that come with
  it) for you to eat food on the go, automatically. Configurable.

- Forbid certain food from being eaten. Configurable.

- Prioritize the food in your inventory’s active row for eating.

- An option to stay in the same facing direction even after eating.

- Increase or decrease eating time intervals for role-playing/challenge
  or performance reasons.

- Options can be configured through the [Generic Config
  Menu](https://www.nexusmods.com/stardewvalley/mods/5098) or
  `config.json` file within the `SelectiveEating` mod directory.

## Version

0.4.3

## Compatibility

If you found this mod behaving weirdly or being incompatible with other
mods, please report it in the
[`bugs`](https://www.nexusmods.com/stardewvalley/mods/26831?tab=bugs) or
[`posts`](https://www.nexusmods.com/stardewvalley/mods/26831?tab=posts)
page so I can update this documentation with that information and to let
other users know about such potential issues in the future.

> This mod should work with most other mods unless specified otherwise
> below.

### Compatible with multiplayer?

Untested.

### Incompatible with

- Fluent Eating

- Auto-eat (including the 1.6 updated version)

## Known issues

- No known issue.

## What changed?

See
[CHANGELOG.md](https://github.com/cloudyluna/StardewValleyMods/tree/main/SelectiveEating/CHANGELOG.md)
file for details.

## Requirements

- Stardew Valley 1.6 (preferably, version 1.6.8)

- [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400) (minimum
  v4.0.0 or higher)

- [Generic Mod Config
  Menu](https://www.nexusmods.com/stardewvalley/mods/5098) - *Optional*
  but *highly recommended*.

## For users

### Installing SelectiveEating

1.  This mod requires SMAPI, so please, install that and other listed
    requirements first.

2.  Unzip the files into the **Stardew Valley/Mods/SelectiveEating**
    game folder.

3.  Make sure the **SelectiveEating.dll**, **CloudyCore.dll**,
    **FSharp.Core.dll**, and **manifest.json** files are inside
    **SelectiveEating** folder, not in **Mods** folder.

4.  Launch the game through SMAPI launcher.

## For developers

### Building SelectiveEating

> Attention: Make sure you already have set up \$*GAME_PATH* pointing to
> your Stardew Valley game folder first.

If you cloned this project from the Github and have `nix` installed, run
`nix develop` to load the development environment. Then run `make` or
`dotnet build` to build the project.

            git clone https://github.com/cloudyluna/StardewValleyMods
            cd SelectiveEating
            make release

## Thanks to

- [SMAPI dev and contributors](https://github.com/Pathoschild/SMAPI) for
  making Stardew Valley modding accessible.

- [Generic Config Menu dev and
  contributors](https://www.nexusmods.com/stardewvalley/mods/5098) for
  making mod configuration through GUI, simple and easy.

## Mod page

- [Nexusmods](https://www.nexusmods.com/stardewvalley/mods/26831)

## Source code

- [Github](https://github.com/cloudyluna/StardewValleyMods/tree/main/SelectiveEating)

## License

> Copyright (c) 2024 Cloudyluna

This project is licensed under the AGPL-3.0-or-later license - see the
`LICENSE` file for details.

- The bundled FSharp.Core is licensed under the MIT license. See
  `sublicenses/FSharp.MIT` file for details.
