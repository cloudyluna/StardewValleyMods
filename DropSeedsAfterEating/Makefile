DOTNET = dotnet
DOTNET_VERSION = net6.0
DIST_DIR = bin
DEBUG_DIR = $(DIST_DIR)/Debug/$(DOTNET_VERSION)
RELEASE_DIR = $(DIST_DIR)/Release/$(DOTNET_VERSION)
FOLDER_NAME = DropSeedsAfterEating
FOLDER_DIST_DIR = $(DIST_DIR)/$(FOLDER_NAME)

all: translation 
	$(DOTNET) build

translation:
	mkdir -p $(RELEASE_DIR)
	mkdir -p $(DEBUG_DIR)
	cp -r data/i18n $(RELEASE_DIR)
	cp -r data/i18n $(DEBUG_DIR)

# quick
q:  translation
	$(DOTNET) build --no-restore

documentation:
	make documentation -C ..

release:
	rm -rf bin/ obj/
	$(DOTNET) clean
	$(DOTNET) build --configuration Release
	cp -r ./manifest.json $(RELEASE_DIR)
	mkdir -p $(FOLDER_DIST_DIR)/
	mkdir -p $(RELEASE_DIR)
	mkdir -p $(DEBUG_DIR)
	cp -r data/i18n $(RELEASE_DIR)
	cp -r data/i18n $(DEBUG_DIR)

	cp -f README.md $(RELEASE_DIR)
	cp -f README.md $(DEBUG_DIR)

	cp -f ../LICENSE $(RELEASE_DIR)
	cp -f ../LICENSE $(DEBUG_DIR)

	rm -f $(RELEASE_DIR)/$(FOLDER_NAME).deps.json


	cp -r $(RELEASE_DIR)/* $(FOLDER_DIST_DIR)/
	cd $(DIST_DIR) && zip -9 -r $(FOLDER_NAME).zip $(FOLDER_NAME)

clean:
	$(DOTNET) clean
