#show link: underline

#let mk-section(title, items) = {
  [=== #title
    #list(..items.map(item => [#item]))
  ]
}

#let added(items) = {
  mk-section("Added", items)
}

#let changed(items) = {
  mk-section("Changed", items)
}

#let deprecated(items) = {
  mk-section("Deprecated", items)
}

#let removed(items) = {
  mk-section("Removed", items)
}

#let fixed(items) = {
  mk-section("Fixed", items)
}

#let security(items) = {
  mk-section("Security", items)
}


#let header(description: "") = {
  [= Changelog
    #line(length: 100%)
    All notable changes to this project will be documented in this file.
    \ \
    The format is based on #link("https://keepachangelog.com/en/1.1.0/")[Keep A Changelog], and this project adheres to #link("https://semver.org/spec/v2.0.0.html")[Semantic Versioning].

    #description
  ]
}

#let new-version(version, dt, items) = {
  let normalize(m) = { if m < 10 [0#m] else [#m] }
  let dt = if (
    dt == none
  ) [] else [\- #dt.year\-#normalize(dt.month)\-#dt.day]

  [== \[#version\] #dt
    #line(length: 100%)
    #items.join()]
}

#let unreleased(items) = {
  new-version("Unreleased", none, items)
}

// example use case
/*
#header()

#unreleased((
  added(("new cool thing", "whatever that is")),
  fixed(("mewp new", "blah")),
  deprecated(("Deprecate abc",)),
))

#new-version("0.1.0", (year: 2026, month: 10, day: 17), (
  added(("Added a new cool code",)),
  removed(("Removed new", "blah")),
))

#new-version("0.2.0", (year: 2021, month: 11, day: 14), (
  fixed(("ahah",)),
))
*/
