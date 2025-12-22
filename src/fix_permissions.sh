#!/usr/bin/env bash
# Fix permissions and remove build artifacts in repository src folder
set -euo pipefail

HERE=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
echo "Running fix_permissions in $HERE"

# Determine the user to own files. If running inside devcontainer, use 'vscode' fallback to current user.
TARGET_USER=${TARGET_USER:-vscode}
TARGET_GROUP=${TARGET_GROUP:-vscode}

echo "chown -R ${TARGET_USER}:${TARGET_GROUP} $HERE"
sudo chown -R "${TARGET_USER}:${TARGET_GROUP}" "$HERE"

echo "Removing obj/ and bin/ directories under $HERE"
find "$HERE" -type d \( -name obj -o -name bin \) -prune -exec rm -rf '{}' + || true

echo "Permissions fixed and build artifacts removed under $HERE"
