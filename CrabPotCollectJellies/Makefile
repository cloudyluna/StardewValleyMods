DOTNET = dotnet
DOTNET_VERSION = net6.0
DIST_DIR = bin
DEBUG_DIR = $(DIST_DIR)/Debug/$(DOTNET_VERSION)
RELEASE_DIR = $(DIST_DIR)/Release/$(DOTNET_VERSION)
FOLDER_NAME = CrabPotCollectJellies
FOLDER_DIST_DIR = $(DIST_DIR)/$(FOLDER_NAME)
TRANSLATION_DIR = data/i18n
DOCS_COMPILE = cd docs && pandoc -t gfm

all: translation
	dotnet tool restore
	$(DOTNET) build

translation:
	mkdir -p $(RELEASE_DIR)
	mkdir -p $(DEBUG_DIR)
	mkdir -p $(TRANSLATION_DIR)
	cp -r $(TRANSLATION_DIR) $(RELEASE_DIR)
	cp -r $(TRANSLATION_DIR) $(DEBUG_DIR)

# quick
q:  translation
	$(DOTNET) build --no-restore

documentation:
	$(DOCS_COMPILE) README.tex -o ../README.md
	$(DOCS_COMPILE) CHANGELOG.tex -o ../CHANGELOG.md

documentation_plain:
	cd docs && pandoc -t plain README.tex -o ../README.txt

release:
	rm -rf bin/ obj/
	$(DOTNET) clean
	$(DOTNET) build --configuration Release
	cp -r ./manifest.json $(RELEASE_DIR)
	mkdir -p $(FOLDER_DIST_DIR)/
	mkdir -p $(RELEASE_DIR)
	mkdir -p $(DEBUG_DIR)
	mkdir -p $(TRANSLATION_DIR)

	cp -r $(TRANSLATION_DIR) $(RELEASE_DIR)
	cp -r $(TRANSLATION_DIR) $(DEBUG_DIR)

	cp -f README.md $(RELEASE_DIR)
	cp -f README.md $(DEBUG_DIR)


	rm -f $(RELEASE_DIR)/$(FOLDER_NAME).deps.json


	cp -r $(RELEASE_DIR)/* $(FOLDER_DIST_DIR)/
	cd $(DIST_DIR) && zip -9 -r $(FOLDER_NAME).zip $(FOLDER_NAME)

clean:
	$(DOTNET) clean
