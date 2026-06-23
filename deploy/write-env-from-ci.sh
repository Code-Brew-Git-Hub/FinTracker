#!/usr/bin/env bash
# Создаёт .env для server-деплоя из переменных GitLab CI/CD.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

if [[ -z "${DB_PASSWORD:-}" ]]; then
  echo "ERROR: задайте переменную DB_PASSWORD в GitLab → Settings → CI/CD → Variables"
  exit 1
fi

if [[ -z "${LETSENCRYPT_EMAIL:-}" ]]; then
  echo "ERROR: задайте переменную LETSENCRYPT_EMAIL в GitLab → Settings → CI/CD → Variables"
  exit 1
fi

DOMAIN="${DOMAIN:-fintracker.trydov1k.online}"
PUBLIC_URL="${PUBLIC_URL:-https://${DOMAIN}}"

cat > .env <<EOF
DEPLOY_MODE=server
DB_NAME=${DB_NAME:-FinTrackerDb}
DB_USER=${DB_USER:-postgres}
DB_PASSWORD=${DB_PASSWORD}
DOMAIN=${DOMAIN}
PUBLIC_URL=${PUBLIC_URL}
LETSENCRYPT_EMAIL=${LETSENCRYPT_EMAIL}
GHCR_OWNER=${GHCR_OWNER:-code-brew-git-hub}
FINTRACKER_VERSION=${FINTRACKER_VERSION:-latest}
EOF

echo ">> .env created for server deploy (${DOMAIN})"
