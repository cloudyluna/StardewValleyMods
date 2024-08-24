DOTNET = dotnet
PANDOC = pandoc
DOCS_DIR = docs
PFLAGS = cd $(DOCS_DIR) && $(PANDOC) -t gfm

all: build

build:
	make -C SelectiveEating
	make -C IdenticalIndoorGlowRingRadius

documentation: $(DOCS_DIR)
	$(PFLAGS) Main.tex -o ../README.md
	$(PFLAGS) SelectiveEating.tex -o \
	   ../SelectiveEating/README.md
