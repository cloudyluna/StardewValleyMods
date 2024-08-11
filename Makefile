DOTNET = dotnet6
PANDOC = pandoc
DOCS = ./docs
PFLAGS = $(PANDOC) -t commonmark $(DOCS)
TARGET = " "

all: build

build:
	$(DOTNET) build 

build-project:
	$(DOTNET) build $(TARGET)

run-project:
	$(DOTNET) run --project $(TARGET)

documentation: $(DOCS)
	$(PFLAGS)/Main.tex -o README.md
	$(PFLAGS)/SelectiveEating.tex -o ./SelectiveEating/README.md
