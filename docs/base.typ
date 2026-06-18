#show link: underline

#let author = "Momo"

// Functions.

#let mk-project-title(title, description) = {
  [= #title
    #line(length: 100%)

    #description
    \ \
  ]
}

#let mk-project-version(version) = {
  [== Version
    #line(length: 100%)

    #version
  ]
}

#let mk-mod-link(mod-uid, name, tab: "description") = {
  let url = str(
    "https://www.nexusmods.com/stardewvalley/mods/"
      + str(mod-uid),
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

#let mk-changelog-notice(mod-name) = {
  [== What changed?
    #line(length: 100%)

    See #link("https://codeberg.org/mistymomo/StardewValleyMods/src/branch/main/" + mod-name + "/CHANGELOG.md") file for details.]
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

    #mk-changelog-notice(mod-name)]
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
        "https://github.com/cloudyluna/StardewValleyMods/tree/main/"
          + mod-name,
        "Github",
      )
  ] else []

  [== Source code
    #line(length: 100%)

    #github

    - #link(
        "https://codeberg.org/mistymomo/StardewValleyMods/src/branch/main/"
          + mod-name,
        "Codeberg",
      )
  ]
}


#let mk-mod-page-link(mod-uid) = {
  [== Mod page
    #line(length: 100%)

    #mk-mod-link(mod-uid, "Description")
  ]
}


#let thanks-to = [
  == Thanks to
  #line(length: 100%)

  - #link("https://github.com/Pathoschild/SMAPI", "SMAPI dev and contributors") for making Stardew Valley modding accessible.

  - #link("https://www.nexusmods.com/stardewvalley/mods/5098", "Generic Config Menu dev and contributors") for making mod configuration through GUI, simple and easy.
]
