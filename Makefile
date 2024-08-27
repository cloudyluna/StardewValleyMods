DOTNET = dotnet
PANDOC = pandoc
DOCS_DIR = docs
PFLAGS = cd $(DOCS_DIR) && $(PANDOC) -t gfm
OUT = README.md

all: build

build:
	make -C SelectiveEating
	make -C MaintainGlowRingsIndoorsRadius

documentation: $(DOCS_DIR)
	$(PFLAGS) Main.tex -o ../$(OUT)
	$(PFLAGS) SelectiveEating.tex -o \
	   ../SelectiveEating/$(OUT)
	$(PFLAGS) MaintainGlowRingsIndoorsRadius.tex -o \
	   ../MaintainGlowRingsIndoorsRadius/$(OUT)
