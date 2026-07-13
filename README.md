# Jellyfin Badges

Displays HDR/Dolby Vision, audio format, source, and rating badges on the
Jellyfin item detail page — sourced from Jellyfin's own media stream data
and, for ratings, directly from your NFO files. No external API calls, no
API keys required.

## Features

**Ratings**
- IMDb — read from Jellyfin's `CommunityRating` field
- TMDb — read directly from the NFO file next to each video, since
  Jellyfin's own API only exposes a single collapsed rating value, not
  per-source ratings
- Each rating shown as its logo + score, with its own text color
  (IMDb gold, TMDb teal)

**Resolution**
- 4K shown as a logo
- 1080p / 720p shown as text pills (no logo available)

**Codec**
- HEVC, H.264, AV1 shown as text pills

**HDR / Dolby Vision**
- Dolby Vision, HDR10, HDR10+, and generic HDR, each as its own logo
- Dolby Vision automatically suppresses HDR10/HDR10+ badges alongside it,
  since Dolby Vision is already an HDR variant (not configurable)

**Audio**
- Dolby Atmos, Dolby TrueHD, DTS:X, DTS-HD MA, Dolby Digital, Dolby
  Digital+, each as its own logo
- Atmos always shown before TrueHD (not configurable)
- Channel layout (e.g. "7.1") is always hidden when Atmos or DTS:X is
  shown, since both are object-based formats that already imply the
  channel count (not configurable)

**Source**
- UHD Blu-ray and Blu-ray shown as logos, detected from the file path
  (REMUX/BDMV/BLURAY keywords)
- WEB-DL shown as a text pill, detected from the file path

**Configuration**
- Every badge has its own enable checkbox and its own editable logo URL,
  sitting right next to each other in a table, grouped by category
  (Ratings / Resolution / HDR / Audio / Source / General)
- Poll interval and debug logging are also configurable
- All settings editable from Dashboard > Plugins > Jellyfin Badges,
  no code changes needed

## Install

1. Dashboard > Plugins > Repositories > add
   `https://raw.githubusercontent.com/spidey58/jellyfin-badges-plugin/main/manifest.json`
2. Catalog > install Jellyfin Badges > restart Jellyfin
3. Recommended: also install File Transformation (see below) for
   permission-free script injection in Docker
4. Dashboard > Plugins > Jellyfin Badges to configure logo URLs and
   toggle individual badges

## Recommended: install File Transformation

Jellyfin Badges needs to add a small script tag to Jellyfin's web client.
The cleanest way to do this - especially in Docker, where the web root is
often not writable by the Jellyfin process - is via the community
**File Transformation** plugin, which lets other plugins modify served
files in memory without touching disk.

1. Dashboard > Plugins > Repositories > add
   `https://www.iamparadox.dev/jellyfin/plugins/manifest.json`
2. Catalog > find **File Transformation** > install > restart Jellyfin

If File Transformation isn't installed, Jellyfin Badges falls back to
writing the script tag directly into `index.html` on disk, which can fail
with a permission error in Docker (visible in Dashboard > Logs as
`Access to the path '.../index.html' is denied`).

### Alternative: Docker volume mount

If you'd rather not install File Transformation, you can instead give the
Jellyfin container write access to just `index.html`:

```bash
docker cp :/usr/share/jellyfin/web/index.html ./index.html
```

Then add a volume mount in your `docker-compose.yml`:

```yaml
volumes:
  - ./index.html:/usr/share/jellyfin/web/index.html
```

Recreate the container.

## How it works

- A hosted service (`BadgesStartupService`) runs at server startup and
  either registers a transformation with File Transformation, or falls
  back to writing directly to `index.html`
- The injected script (`Badges/script`) fetches its configuration live
  from `Badges/config`, which reflects whatever is set on the Dashboard
  page - no rebuild needed to change settings
- On each item detail page, the script reads `MediaStreams` and
  `CommunityRating` from Jellyfin's own API and renders badges into the
  existing info row, replacing the default star rating
- TMDb ratings are fetched separately via `Badges/nfoRating`, which
  locates and parses the NFO file next to the video file on the server
- The plugin's own config page (`Badges/configScript`) is loaded via a
  bootstrap trick: Jellyfin's dashboard inserts plugin pages via
  `innerHTML`, which never executes `<script>` tags (standard browser
  behavior), so an `<img onerror>` attribute is used instead to
  dynamically create a real `<script src="...">` element, which does
  execute normally

## Development

```bash
dotnet build Jellyfin.Plugin.Badges/Jellyfin.Plugin.Badges.csproj -c Release
```

GitHub Actions (`.github/workflows/build.yml`) builds, packages, and
publishes a release automatically on every push to `main`, and updates
`manifest.json` with the new version's download URL and checksum.

## Logo assets

Default logo URLs point to
[spidey58/jellyfinbadges](https://github.com/spidey58/jellyfinbadges).
Every logo URL is individually editable from the config page, so any
single one can be repointed without affecting the others. Logo
trademarks (Dolby, DTS, TMDb, etc.) belong to their respective owners;
this plugin only links to externally-hosted images and does not bundle
or redistribute them.
