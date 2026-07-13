# Jellyfin Badges

Displays HDR/Dolby Vision, audio format, source, and rating badges on the
Jellyfin item detail page.

## Status: diagnostic build (v1.0.3.0)

The config page is temporarily a minimal diagnostic test, not the full
settings UI, while we isolate why the full config page's inline script
was not executing on this server (Jellyfin 10.11.11). See the heading on
the config page after loading it - if it changes to say
"SCRIPT EXECUTED SUCCESSFULLY", inline scripts work fine here and the
issue was isolated to something else in the fuller version.

## Install

1. Dashboard > Plugins > Repositories > add
   `https://raw.githubusercontent.com/spidey58/jellyfin-badges-plugin/main/manifest.json`
2. Catalog > install Jellyfin Badges > restart Jellyfin
3. Recommended: also install File Transformation (see below) for
   permission-free script injection in Docker

## Recommended: install File Transformation

1. Dashboard > Plugins > Repositories > add
   `https://www.iamparadox.dev/jellyfin/plugins/manifest.json`
2. Catalog > find **File Transformation** > install > restart Jellyfin

## Alternative: Docker volume mount

```bash
docker cp <container_name>:/usr/share/jellyfin/web/index.html ./index.html
```

```yaml
volumes:
  - ./index.html:/usr/share/jellyfin/web/index.html
```

## Development

```bash
dotnet build Jellyfin.Plugin.Badges/Jellyfin.Plugin.Badges.csproj -c Release
```

GitHub Actions builds, packages, and releases automatically on push to
`main`.
