#!/usr/bin/env bash
# Fix permissions and remove build artifacts for infra project (and optionally repo-wide)
set -euo pipefail

HERE=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
echo "Running infra fix_permissions in $HERE"

# By default, operate on the infra project only. To operate repo-wide, pass --all
REPO_ROOT="$(cd "$HERE/.." && pwd)"
TARGETS=("$HERE")

if [[ "${1:-}" == "--all" ]]; then
  echo "Operating across repository root: $REPO_ROOT"
  TARGETS=("$REPO_ROOT/src" "$REPO_ROOT/infra")
fi

# Determine the user to own files. Default to 'vscode' in devcontainer.
TARGET_USER=${TARGET_USER:-vscode}
TARGET_GROUP=${TARGET_GROUP:-vscode}

for T in "${TARGETS[@]}"; do
  if [[ -d "$T" ]]; then
    echo "Fixing ownership under $T"
    sudo chown -R "${TARGET_USER}:${TARGET_GROUP}" "$T" || true
    echo "Removing obj/ and bin/ directories under $T"
    find "$T" -type d \( -name obj -o -name bin \) -prune -exec rm -rf '{}' + || true
  else
    echo "Target not found: $T"
  fi
done

echo "Infra fix_permissions complete"
