DOTNET = dotnet
PANDOC = pandoc

all: build

build:
	make -C SelectiveEating
	make -C MaintainGlowRingsIndoorsRadius
	make -C DropSeedsAfterEating

documentation:
	cd docs && pandoc -t gfm Main.tex -o ../README.md
	make documentation -C SelectiveEating
	make documentation -C MaintainGlowRingsIndoorsRadius
	make documentation -C DropSeedsAfterEating

release:
	make release -C SelectiveEating
	make release -C MaintainGlowRingsIndoorsRadius
	make release -C DropSeedsAfterEating