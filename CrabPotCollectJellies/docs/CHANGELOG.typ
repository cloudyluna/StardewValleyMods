#import "../../docs/base_changelog.typ" as bc

#show link: underline

#bc.mk-changelog[
  #bc.new-version("1.2.1", (year: 2026, month: 2, day: 18), (
    bc.fixed(([Missing white algae spawn in the sewer and the mutant's bug lair.],)),
  ))

  #bc.new-version("1.2.0", (year: 2026, month: 2, day: 18), (
    bc.added(([White algae],)),
    bc.fixed(([Rebalance seaweeds and algae spawn chances.], [Only spawn seaweeds in the beach map.])),
  ))

  #bc.new-version("1.1.0", (year: 2026, month: 2, day: 13), (
    bc.added(([Algae and seaweed collection as optionals.],)),
  ))

  #bc.new-version("1.0.2", (year: 2026, month: 9, day: 18), (
    bc.fixed(([*_Enable mod_* option in GMCM raising errors when unticked.],)),
  ))

  #bc.new-version("1.0.1", (year: 2026, month: 9, day: 16), (
    bc.fixed(([Crab pots unable to collect jelly when player picks \textbf{mariner} skill (fishing level 10).],)),
  ))

  #bc.new-version("1.0.1", (year: 2026, month: 9, day: 16), (
    bc.added((
      [Crab pots in #link("https://stardewvalleywiki.com/The_Mines", "mines/underground caves/dungeons") (level 20, 60, 100) now can collect jellies
        and the only jelly they will catch is cave jelly.
        Farm cave and Witch's swamp *_are not affected_*.],
      [Jellies can be of different quality than normal.],
    )),
    bc.changed((
      [*_Breaking change_:* Treat jellies catch or collect chance like any regular crab pot fish object (clam, mussel, crab, etc) instead of trash.\

        As it seems some players are farming jellies items and uses extra skill perks mod to completely remove trash or increase other
        crab pot fish, catch rate later in the game, I find it probably makes more sense
        to convert jelly from a "trash" item into a crab pot fish one in the long term.],
      [Rebalanced global river and ocean jellies catch rate so they don't always completely overtake
        things like crabs or clams all the time as these items are rarer than trash items. This is made to
        accommodate the change made above.],
    )),
    bc.removed((
      [The option to replace all the trash from configuration menu.\

        Users should move to use other mods that will remove trash spawn chance instead as it is now considered a non-goal and won't be much of use for this project.],
    )),
  ))

  #bc.new-version("0.3.0", (year: 2024, month: 9, day: 4), (
    bc.added((
      [Crab pots in cave farm and witch's swamp area now have much higher chance to collect cave jellies.],
      [10% more chance for crab pots in cave farm than witch's swamp to collect cave jellies.],
    )),
  ))

  #bc.new-version("0.2.0", (year: 2024, month: 8, day: 29), (
    bc.added((
      [Option to replace all trash that will be collected by crab pots with jellies. Configurable with Generic Mod Config Menu.],
      [Option to enable or disable this mod with config menu.],
    )),
  ))

  #bc.new-version("0.1.0", none, (bc.added(([Initial release.],)),))


]
