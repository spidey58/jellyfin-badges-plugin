#!/usr/bin/env python3
"""
Updates manifest.json with a new release entry after each build.
Called from the GitHub Actions workflow.
"""
import argparse
import json
import os
from datetime import datetime, timezone

MANIFEST_PATH = "manifest.json"
PLUGIN_GUID = "2c0f75c3-6aad-4003-95e3-0fd418279ac3"
PLUGIN_NAME = "Jellyfin Badges"
TARGET_ABI = "10.9.0.0"


def load_manifest():
    if os.path.exists(MANIFEST_PATH):
        with open(MANIFEST_PATH, "r", encoding="utf-8") as f:
            return json.load(f)
    return [
        {
            "guid": PLUGIN_GUID,
            "name": PLUGIN_NAME,
            "description": "Displays HDR/Dolby Vision, audio format, source, and rating badges on the detail page.",
            "overview": "HDR/Dolby Vision, audio, source, and rating badges for the detail page.",
            "owner": "spidey58",
            "category": "General",
            "imageUrl": "",
            "versions": [],
        }
    ]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--version", required=True)
    parser.add_argument("--checksum", required=True)
    parser.add_argument("--repo", required=True)
    parser.add_argument("--zip-name", required=True)
    args = parser.parse_args()

    manifest = load_manifest()
    entry = manifest[0]

    source_url = (
        f"https://github.com/{args.repo}/releases/download/"
        f"v{args.version}/{args.zip_name}"
    )

    new_version = {
        "version": args.version,
        "changelog": f"Automated release v{args.version}",
        "targetAbi": TARGET_ABI,
        "sourceUrl": source_url,
        "checksum": args.checksum,
        "timestamp": datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
    }

    entry["versions"] = [
        v for v in entry.get("versions", []) if v.get("version") != args.version
    ]
    entry["versions"].insert(0, new_version)

    with open(MANIFEST_PATH, "w", encoding="utf-8") as f:
        json.dump(manifest, f, indent=4)
        f.write("\n")

    print(f"manifest.json updated with version {args.version}")


if __name__ == "__main__":
    main()
