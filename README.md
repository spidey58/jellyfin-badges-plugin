# Jellyfin Badges

Displays HDR/Dolby Vision, audio format, source, and rating badges on the
Jellyfin item detail page — sourced entirely from data Jellyfin already has
(populated from your NFO metadata via tinyMediaManager or similar tools).
No external API calls, no API keys.

## Features

- **Rating badge** — Community rating (e.g. IMDb) shown as a logo + score
- **Resolution badge** — 4K shown as a logo, 1080p/720p as text pills
- **Codec badge** — HEVC, H.264, AV1
- **HDR badge** — Dolby Vision, HDR10+, HDR10, HDR (Dolby Vision suppresses
  the others by default, since DV is already an HDR variant)
- **Audio badge** — Atmos, TrueHD, DTS:X, DTS-HD MA, Dolby Digital/+ (Atmos
  suppresses the channel layout pill by default, and can be ordered before
  TrueHD)
- **Source badge** — UHD Blu-ray, Blu-ray, or WEB-DL, detected from the
  file path
- All logos and toggles are configurable from **Dashboard > Plugins >
  Jellyfin Badges** — no code editing required

## Install

1. In Jellyfin, go to **Dashboard > Plugins > Repositories**
2. Add a new repository with this URL:
   ```
   https://raw.githubusercontent.com/spidey58/jellyfin-badges-plugin/main/manifest.json
   ```
3. Go to **Catalog**, find **Jellyfin Badges** under General, and install
4. Restart Jellyfin
5. Go to **Dashboard > Plugins > Jellyfin Badges** to configure logo URLs
   and toggle features
6. Hard-refresh the web client (Ctrl+F5)

## How it works

- A server-side entry point (`BadgesEntryPoint`) patches Jellyfin's
  `index.html` on startup to load `Badges/script`
- The client script fetches its configuration from `Badges/config` at
  runtime, which reflects whatever is set in the plugin's Dashboard page
- On each item detail page, the script reads `MediaStreams` and
  `CommunityRating` from Jellyfin's own API and renders badges into the
  existing info row

### Known limitation

The `index.html` patch is reapplied by the plugin on every server start,
but if Jellyfin's web client is reinstalled/updated outside of a normal
server restart cycle, the patch may need to reapply — this happens
automatically on next startup, no manual action needed.

## Development

Build locally:

```bash
dotnet build Jellyfin.Plugin.Badges/Jellyfin.Plugin.Badges.csproj -c Release
```

The GitHub Actions workflow (`.github/workflows/build.yml`) builds,
packages, and publishes a release automatically on every push to `main`,
and updates `manifest.json` with the new version's download URL and
checksum.

## Logo assets

Default logo URLs point to
[spidey58/jellyfinbadges](https://github.com/spidey58/jellyfinbadges).
Logo trademarks (Dolby, DTS, etc.) belong to their respective owners;
this plugin only links to externally-hosted images and does not bundle
or redistribute them.
