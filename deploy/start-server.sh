#!/bin/sh
set -eu

ROOT=$(CDPATH= cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)
cd "$ROOT"

if [ ! -f .env ]; then
  echo ">> Creating .env from .env.example"
  cp .env.example .env
  echo "Edit .env: set DEPLOY_MODE=server, DOMAIN, DB_PASSWORD"
  exit 1
fi

# shellcheck disable=SC1091
set -a
. ./.env
set +a

if [ "${DEPLOY_MODE:-local}" != "server" ]; then
  echo "DEPLOY_MODE must be 'server' in .env"
  exit 1
fi

if [ -z "${DOMAIN:-}" ]; then
  echo "Set DOMAIN in .env"
  exit 1
fi

echo ">> Pulling images"
docker compose -f docker-compose.images.yml -f docker-compose.server.yml pull

echo ">> Starting FinTracker (server)"
docker compose -f docker-compose.images.yml -f docker-compose.server.yml up -d

PUBLIC_URL="${PUBLIC_URL:-https://${DOMAIN}}"
PORT="${FINTRACKER_HOST_PORT:-8082}"
echo ""
echo "FinTracker is running on http://localhost:${PORT}"
echo "Configure Nginx Proxy Manager → ${PUBLIC_URL}"
echo "Stop: docker compose -f docker-compose.images.yml -f docker-compose.server.yml down"
