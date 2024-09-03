# Changelog

## \[0.4.2\]- 2024-09-01

### Changed

- Default condition check interval to 1/3 second.

### Fixed

- A rather major performance issue whenever eating with animation is
  switched on.

- Default priority strategy selection key name not matching with the new
  one.

- Keybind toggle to turn mod on/off not saving things on toggle switch.

- Wonky stamina first food, priority selection.

## \[0.4.1\]

### Added

- Provide a minimal translated string for keybind HUD message.

### Fixed

- Properly use food ID instead of their qualified version (which may
  start with something like (O)91 for banana). Now, we use just number
  91 to block from eating bananas, which doesn’t contradict the given
  example in the config menu.

## \[0.4.0\]

### Added

- Show HUD message when mod’s is toggled on/off with the keybind while
  playing.

- Player condition interval check now has an option to check every 1/3
  of a second. Set this to 0 to make this check occuring far more often.

### Changed

- Forbidden list to take (global) qualified item ids only, which I
  believe is much more accurate to represent item type. One of the site
  to browse the item ids can be found here - https://stardewids.com/.

### Fixed

- Cheapest food option priority. A silly typo broke this whole logic. My
  silly self has fixed and added a bit more code to prevent this from
  happening again.

## \[0.3.0\]

### Added

- New feature to entirely skip eating animation (and the world pauses
  that comes with it) for you to eat food on the go, automatically.
  Configurable.

### Fixed

- Stay in the last facing direction option. I forgot to handle this
  internally..

## \[0.2.1\]

### Fixed

- Forbidden list name when spaces are in use.

## \[0.2.0\]

### Added

- Option to select cheapest food to eat.

- Improve Generic Config Menu settings page with more usage
  explanations.

### Fixed

- Minor performance improvements.

- Forbidden list input now can deal with stray whitespaces and commas.
  Also the names doesn’t have to be case-sensitive.

### Removed

- (Breaking): prioritized food input list and instead I made the active
  inventory row as the as the highest food prioritization for consuming
  (left to right, depending on your configured "eating priority"
  setting).

## \[0.1.0\]

- Initial release.
