DOTNET = dotnet
RELEASE_DIR = ./bin/Release/net6.0/
FOLDER_NAME = SelectiveEating
DIST_DIR = ./bin
FOLDER_DIST_DIR = $(DIST_DIR)/$(FOLDER_NAME)

all: translation 
	$(DOTNET) build

metas:
	cp -f README.md ./bin/Release/net6.0/
	cp -f README.md ./bin/Debug/net6.0/

	cp -f LICENSE ./bin/Debug/net6.0/
	cp -f LICENSE ./bin/Debug/net6.0/

translation:
	mkdir -p ./bin/Release/net6.0/
	mkdir -p ./bin/Debug/net6.0/
	cp -r data/i18n ./bin/Release/net6.0/
	cp -r data/i18n ./bin/Debug/net6.0/

# quick
q:  translation
	$(DOTNET) build --no-restore

release:
	rm -rf bin/ obj/
	$(DOTNET) clean
	$(DOTNET) build --configuration Release
	cp -r ./manifest.json $(RELEASE_DIR)/
	cp -r ./sublicenses/ $(RELEASE_DIR)/
	mkdir -p $(FOLDER_DIST_DIR)/
	
	mkdir -p ./bin/Release/net6.0/
	mkdir -p ./bin/Debug/net6.0/
	cp -r data/i18n ./bin/Release/net6.0/
	cp -r data/i18n ./bin/Debug/net6.0/

	cp -f README.md ./bin/Release/net6.0/
	cp -f README.md ./bin/Debug/net6.0/

	cp -f ../LICENSE ./bin/Release/net6.0/
	cp -f ../LICENSE ./bin/Debug/net6.0/

	rm -f $(RELEASE_DIR)/SelectiveEating.deps.json
	
	
	cp -r $(RELEASE_DIR)/* $(FOLDER_DIST_DIR)/
	cd $(DIST_DIR) && zip -9 -r $(FOLDER_NAME).zip $(FOLDER_NAME)

clean:
	$(DOTNET) clean
