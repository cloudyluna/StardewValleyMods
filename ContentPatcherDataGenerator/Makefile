DOTNET = dotnet6
TARGET = ""

all: build

fresh:
	$(DOTNET) build

install: run
	cp -r "json/$(TARGET)" "$(STARDEW_VALLEY)Mods/"

release: clean run
	cd ./json && zip -9 -r "$(TARGET)".zip "$(TARGET)"
	mv "./json/$(TARGET).zip" ./bin/


build:
	$(DOTNET) build --no-restore

run:
	$(DOTNET) run

clean:
	$(DOTNET) clean
	rm -rf bin/ obj/