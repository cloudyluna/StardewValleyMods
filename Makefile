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
	$(MAKE) documentation -C CrabPotCollectJellies

release: test
	$(MAKE) release -C SelectiveEating
	$(MAKE) release -C MaintainGlowRingsIndoorsRadius
	$(MAKE) release -C DropSeedsAfterEating
	$(MAKE) release -C CrabPotCollectJellies
