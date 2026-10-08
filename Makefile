CONFIGURATION ?= Release
PACKAGE_VERSION ?=
API_KEY ?=

# Argumente für dotnet pack dynamisch zusammenbauen
PACK_ARGS = pack src/RoseLeDark.Collections.Missings.csproj --configuration $(CONFIGURATION) --no-build --output build/publish /p:ContinuousIntegrationBuild=true
ifneq ($(PACKAGE_VERSION),)
PACK_ARGS += /p:PackageVersion=$(PACKAGE_VERSION)
endif

.PHONY: all clean restore build pack

all: pack

clean:
	@echo "=== Clean ==="
	@rm -f build/publish/*.nupkg build/publish/*.snupkg

restore: clean
	@echo "=== Restore ==="
	dotnet restore

build: restore
	@echo "=== Build ==="
	dotnet build SystemEx.slnx --configuration $(CONFIGURATION) --no-restore
	@rm -rf bin/

pack: build
	@echo "=== Pack ==="
	dotnet $(PACK_ARGS)

push:
	@if [ -z "$(API_KEY)" ]; then \
		echo "Fehler: API_KEY ist erforderlich. Aufruf: make push API_KEY=dein_key"; \
		exit 1; \
	fi
	@echo "=== Push ==="
	
	dotnet nuget push "build/publish/*.nupkg" --api-key $(API_KEY) --source "https://api.nuget.org/v3/index.json" --skip-duplicate
	dotnet nuget push "build/publish/*.snupkg" --api-key $(API_KEY) --source "https://api.nuget.org/v3/index.json" --skip-duplicate