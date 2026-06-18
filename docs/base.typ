#show link: underline

#let author = "Momo"

// Functions.

#let mk-title(title, description) = {
  [= #title
    #line(length: 100%)

    #description
    \ \
  ]
}

#let mk-version(version) = {
  [== Version
    #line(length: 100%)

    #version
  ]
}

#let mk-features(version, features) = {
  [== Features
    #line(100%)

    #(for i in features [#i])

    #mk-version(version)
  ]
}

#let mk-deps(additional-deps) = {
  [== Requirements
    #line(length: 100%)

    - Stardew Valley 1.6 (preferably, version 1.6.15)
    - #link("https://www.nexusmods.com/stardewvalley/mods/2400", "SMAPI") (minimum v4.0.0 and higher)
    #(for i in additional-deps [])
  ]
}

#let mk-mod-link(mod-uid, name, tab: "description") = {
  let url = str(
    "https://www.nexusmods.com/stardewvalley/mods/" + str(mod-uid),
  )
  let name = if name == none [#url] else [#name]

  [#link(url + "?tab=" + tab, name)
  ]
}

#let mk-compatibility(mod-uid, incompats, mult-compat-desc) = {
  [== Compatibility
    #line(length: 100%)

    If you found this mod behaving weirdly or being incompatible with other mods, please report it on the #mk-mod-link(mod-uid, "Bugs", tab: "bugs") or #mk-mod-link(mod-uid, "Posts", tab: "posts") page so I can update this documentation with that information and let other users know about such potential issues in the future.

    #emph[This mod should work with most other mods unless specified otherwise below.]

    === Compatible with multiplayer?
    #line(length: 100%)

    #mult-compat-desc

    #(
      if incompats.len() == 0 [] else [==== Incompatible with
        #line(length: 100%)

        #(for i in incompats [- #i])
      ]
    )
  ]
}

#let mk-known-issues(issues) = {
  [== Known issues
    #line(length: 100%)

    #(for i in issues [- #i])
  ]
}

#let mk-install-instructions(mod-name) = {
  [== For users
    === Installing #mod-name

    + This mod requires SMAPI, so please, install that and other listed requirements first.
    + Unzip the files into the *Stardew Valley/Mod/#mod-name* game folder.
    + Make sure the *#mod-name\.dll* and *manifest.json* files are inside the *#mod-name* folder, not in *Mods* folder.
    + Launch the game through SMAPI launcher.
  ]
}

#let mk-changelog(mod-name) = {
  [== What has changed?
    #line(length: 100%)

    See the #link("https://codeberg.org/mistymomo/StardewValleyMods/src/branch/main/" + mod-name + "/CHANGELOG.md", "CHANGELOG.md") file for details.]
}

#let mk-compat-group(
  mod-uid,
  mod-name,
  incompats: (),
  mult-compat: "Untested",
  known-issues: (),
) = {
  [#mk-compatibility(mod-uid, incompats, mult-compat)

    #(
      if known-issues.len() == 0 [] else [#mk-known-issues(
        known-issues,
      )]
    )

    #mk-changelog(mod-name)]
}

#let mk-copyright(year) = {
  let year = if year == none [#(
    datetime.today().year()
  )] else [#year]

  emph[Copyright (c) #year #author]
}

#let mk-license(year: none) = {
  [== License
    #line(length: 100%)

    #mk-copyright(year) \ \
    This project is licensed under the AGPL-3.0-or-later license - see the *#emph[LICENSE]* for details.]
}

#let mk-repo-links(mod-name, sites: (github: false)) = {
  let github = if sites.github [
    - #link(
        "https://github.com/cloudyluna/StardewValleyMods/tree/main/" + mod-name,
        "Github",
      )
  ] else []

  [== Source code
    #line(length: 100%)

    - #link(
        "https://codeberg.org/mistymomo/StardewValleyMods/src/branch/main/" + mod-name,
        "Codeberg",
      )

    #github
  ]
}


#let mk-mod-page-link(mod-uid) = {
  [== Mod page
    #line(length: 100%)

    #mk-mod-link(mod-uid, "Nexusmods")
  ]
}


#let thanks-to = [
  == Thanks to
  #line(length: 100%)

  - #link("https://github.com/Pathoschild/SMAPI", "SMAPI dev and contributors") for making Stardew Valley modding accessible.

  - #link("https://www.nexusmods.com/stardewvalley/mods/5098", "Generic Config Menu dev and contributors") for making mod configuration through GUI, simple and easy.
]

#let mk-acknowledge(mod-uid, mod-name, repo-sites) = {
  [
    #thanks-to

    #mk-mod-page-link(mod-uid)

    #mk-repo-links(mod-name, sites: repo-sites)
  ]
}
