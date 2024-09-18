include ../Makefile.variable

PROJECT_NAME:=CrabPotCollectJellies
VERSION:=1.0.2

.PHONY: release

release:
	rm -rf bin/ obj/
	$(DOTNET) clean
	$(UPDATE_VERSIONS)
	$(DOTNET) build --configuration Release

	cp -r ./manifest.json $(RELEASE_DIR)
	mkdir -p $(FOLDER_DIST_DIR)/

	mkdir -p $(RELEASE_DIR)
	mkdir -p $(DEBUG_DIR)

	cp -r $(TRANSLATION_DIR) $(RELEASE_DIR)
	cp -r $(TRANSLATION_DIR) $(DEBUG_DIR)

	$(DOCS_COMPILE) README.tex -o ../README.md
	$(DOCS_COMPILE) CHANGELOG.tex -o ../CHANGELOG.md
	cp -f README.md $(RELEASE_DIR)
	cp -f CHANGELOG.md $(RELEASE_DIR)
	cp -f README.md $(DEBUG_DIR)
	cp -f CHANGELOG.md $(DEBUG_DIR)

	rm -f $(RELEASE_DIR)/$(PROJECT_NAME).deps.json

	cp -r $(RELEASE_DIR)/* $(FOLDER_DIST_DIR)/
	cd $(DIST_DIR) && zip -r $(PROJECT_NAME).zip $(PROJECT_NAME)