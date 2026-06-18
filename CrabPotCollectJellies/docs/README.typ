#import "../../docs/base.typ" as base

#show link: underline

#let mod-title = "Crab Pot Collect Jellies"
#let mod-name = mod-title.replace(regex("\s+"), "")

#let mod-version = "1.2.1"
#let mod-uid = 27358

#base.mk-title(
  mod-title,
  [Allow crab pots to collect aquatic jellies.\ \

    This mod uses the same crab pot's fish (like crabs and clams) catch chance value to collect jellies randomly. The spawn type and in which area are as follows:

    - Sea Jelly -- Ocean tiles; by Willy's shop or the beach, etc.
    - River Jelly -- Rivers, lakes, sewers, etc.
    - Witch's swamp and farm cave (if you have farm cave + water map mod installed). You also can trap cave jellies from rivers,
      lakes, sewers, etc. but they are much rarer there.
  ],
)

#base.mk-version(mod-version)

== Questions that might come up as you use this mod
#line(length: 100%)

=== How does this work?

This mod only intercepts #link("https://stardewvalleywiki.com/Fish#Crab_Pot_Fish", "crab pot's fish")
catch rate and adds jellies to them.
Skills, perks, luck and anything else that affects this rate will also affect how often (or not)
you'll catch aquatic jellies with the crab pots.

=== What's the best place to trap more cave jellies?

#emph[Reminder: This mod does not allow you to put crab pots in the mines. You have to install other mods to do that.]

- In the vanilla game, your best luck to get more cave jellies will be in the #link("https://stardewvalleywiki.com/Witch%27s_Swamp", "Witch's swamp") location.
  The chance is about 10% to 20% higher for cave jellies here.

- In a modded game, if your #link("https://stardewvalleywiki.com/The_Cave", "Farm Cave") have water tiles of which you could place crab pots in, then you should get 10% higher chance to get jellies compared to the one in Witch's swamp.

  Recommended farm mod to use: #link("https://www.nexusmods.com/stardewvalley/mods/23190", "More Lively Farm Cave")

- #link("https://stardewvalleywiki.com/Mines", "Mines") (level 20, 60, 100) if it's possible to put crab pots in those water tiles.
  The chance is the same as farm cave one above, except that no river or sea jelly will spawn here.

=== How to get better quality jelly?

The exact same as you would do to get any other high quality, crab pot's fish item.
Use better bait.

=== Does this mod changes the actual jelly item?

Not at all. It only alters the catch rate, specifically to be collected from crab pots.


=== Does this mod work in location X?

If your crab pots can collect things like crabs, mussels, lobsters and similar items in said location,
then they definitely can collect aquatic jellies there too.

#base.mk-compat-group(mod-uid, mod-name, incompats: ("Any mod that drastically changes crab pot mechanism.",))

#base.mk-deps(())

#base.mk-install-instructions(mod-name)

#base.mk-acknowledge(mod-uid, mod-name, (github: true))

#base.mk-license(year: 2026)

#emph[
  This license does not include the copied Stardew's own code.
]
