PANDOC = pandoc

all: build

build:
	$(MAKE) -C SelectiveEating
	$(MAKE) -C MaintainGlowRingsIndoorsRadius
	$(MAKE) -C DropSeedsAfterEating
	$(MAKE) -C CrabPotCollectJellies

test:
	$(MAKE) test -C SelectiveEating

documentation:
	cd docs && pandoc -t gfm Main.tex -o ../README.md
	$(MAKE) documentation -C SelectiveEating
	$(MAKE) documentation -C MaintainGlowRingsIndoorsRadius
	$(MAKE) documentation -C DropSeedsAfterEating
	$(MAKE) documentation -C CrabPotCollectJellies

release: test
	$(MAKE) release -C SelectiveEating
	$(MAKE) release -C MaintainGlowRingsIndoorsRadius
	$(MAKE) release -C DropSeedsAfterEating
	$(MAKE) release -C CrabPotCollectJellies
