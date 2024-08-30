DOTNET = dotnet
PANDOC = pandoc

all: build

build:
	$(MAKE) -C SelectiveEating
	$(MAKE) -C MaintainGlowRingsIndoorsRadius
	$(MAKE) -C DropSeedsAfterEating

documentation:
	cd docs && pandoc -t gfm Main.tex -o ../README.md
	$(MAKE) documentation -C SelectiveEating
	$(MAKE) documentation -C MaintainGlowRingsIndoorsRadius
	$(MAKE) documentation -C DropSeedsAfterEating

release:
	$(MAKE) release -C SelectiveEating
	$(MAKE) release -C MaintainGlowRingsIndoorsRadius
	$(MAKE) release -C DropSeedsAfterEating