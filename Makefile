DOTNET = dotnet
PANDOC = pandoc
DOCS_DIR = docs
PFLAGS = cd $(DOCS_DIR) && $(PANDOC) -t gfm
TARGET = " "

all: build

build:
	make -C SelectiveEating

documentation: $(DOCS_DIR)
	$(PFLAGS) Main.tex -o ../README.md
	$(PFLAGS) SelectiveEating.tex -o \
	   ../SelectiveEating/README.md
