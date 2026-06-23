#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

echo ">> Stopping FinTracker (server)"
docker compose -f docker-compose.images.yml -f docker-compose.server.yml down

echo "Done. PostgreSQL data is kept (volume postgres_data)."
