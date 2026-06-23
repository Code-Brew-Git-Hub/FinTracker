#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

if [[ ! -f .env ]]; then
  echo ">> Creating .env from .env.example"
  cp .env.example .env
  echo "Edit .env: set DEPLOY_MODE=server, DOMAIN, LETSENCRYPT_EMAIL, DB_PASSWORD"
  exit 1
fi

# shellcheck disable=SC1091
source <(grep -E '^\s*[A-Za-z_][A-Za-z0-9_]*=' .env | sed 's/^\s*//')

DEPLOY_MODE="${DEPLOY_MODE:-local}"
if [[ "$DEPLOY_MODE" != "server" ]]; then
  echo "DEPLOY_MODE must be 'server' in .env (current: $DEPLOY_MODE)"
  exit 1
fi

if [[ -z "${DOMAIN:-}" || -z "${LETSENCRYPT_EMAIL:-}" ]]; then
  echo "Set DOMAIN and LETSENCRYPT_EMAIL in .env"
  exit 1
fi

echo ">> Pulling images"
docker compose -f docker-compose.images.yml -f docker-compose.server.yml pull

echo ">> Starting FinTracker (server)"
docker compose -f docker-compose.images.yml -f docker-compose.server.yml up -d

PUBLIC_URL="${PUBLIC_URL:-https://${DOMAIN}}"
echo ""
echo "FinTracker is running."
echo "  Site: $PUBLIC_URL"
echo "Stop: docker compose -f docker-compose.images.yml -f docker-compose.server.yml down"
