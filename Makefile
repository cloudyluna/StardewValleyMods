PANDOC = pandoc
DOCS = ./docs
PFLAGS = $(PANDOC) -t commonmark $(DOCS)

documentation: $(DOCS)
	$(PFLAGS)/Main.tex -o README.md
	$(PFLAGS)/SelectiveEating.tex -o ./SelectiveEating/README.md