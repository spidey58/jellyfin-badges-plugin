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

- A hosted service (`BadgesStartupService`) runs at server startup and
  arranges for `index.html` to load `Badges/script`
- **Preferred method:** if the community
  [File Transformation](https://github.com/IAmParadox27/jellyfin-plugin-file-transformation)
  plugin is installed, this plugin registers a transformation with it that
  injects the script tag into `index.html` at request time, in memory,
  with no disk writes. This avoids permission errors entirely and is
  strongly recommended, especially for Docker deployments.
- **Fallback method:** if File Transformation is not installed, this
  plugin writes the script tag directly into `index.html` on disk. This
  can fail with a permission error in Docker if the web root isn't
  writable by the Jellyfin process - if you see
  `Access to the path '.../index.html' is denied` in
  **Dashboard > Logs**, install File Transformation from the plugin
  catalog instead of trying to fix container permissions.
- The client script fetches its configuration from `Badges/config` at
  runtime, which reflects whatever is set in the plugin's Dashboard page
- On each item detail page, the script reads `MediaStreams` and
  `CommunityRating` from Jellyfin's own API and renders badges into the
  existing info row

### Recommended: install File Transformation

1. Dashboard > Plugins > Repositories > add
   `https://www.iamparadox.dev/jellyfin/plugins/manifest.json`
2. Catalog > find **File Transformation** > install > restart Jellyfin
3. No configuration needed - Jellyfin Badges detects and uses it
   automatically on next startup

### Alternative: Docker volume mount

If you'd rather not install another plugin, you can instead give the
Jellyfin container write access to just `index.html`:

```bash
docker cp <container_name>:/usr/share/jellyfin/web/index.html ./index.html
```

Then add a volume mount in your `docker-compose.yml` (adjust the source
path and container web path to match your setup):

```yaml
volumes:
  - ./index.html:/usr/share/jellyfin/web/index.html
```

Recreate the container. This gives the file the container's own
ownership instead of the read-only image layer's, so the plugin's
fallback direct-write method can succeed.

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
