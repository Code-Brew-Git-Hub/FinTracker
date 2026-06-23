#!/bin/sh
# Создаёт .env для server-деплоя из переменных GitLab CI/CD.
set -eu

ROOT=$(CDPATH= cd "$(dirname "$0")/.." && pwd)
cd "$ROOT"

if [ -z "${DB_PASSWORD:-}" ]; then
  echo "ERROR: задайте переменную DB_PASSWORD в GitLab → Settings → CI/CD → Variables"
  exit 1
fi

if [ -z "${LETSENCRYPT_EMAIL:-}" ]; then
  echo "ERROR: задайте переменную LETSENCRYPT_EMAIL в GitLab → Settings → CI/CD → Variables"
  exit 1
fi

DOMAIN="${DOMAIN:-fintracker.trydov1k.online}"
PUBLIC_URL="${PUBLIC_URL:-https://${DOMAIN}}"

# Значения в двойных кавычках — пароль может содержать $, # и другие символы.
{
  printf 'DEPLOY_MODE=server\n'
  printf 'DB_NAME="%s"\n' "${DB_NAME:-FinTrackerDb}"
  printf 'DB_USER="%s"\n' "${DB_USER:-postgres}"
  printf 'DB_PASSWORD="%s"\n' "$(printf '%s' "$DB_PASSWORD" | sed 's/"/\\"/g')"
  printf 'DOMAIN="%s"\n' "$DOMAIN"
  printf 'PUBLIC_URL="%s"\n' "$PUBLIC_URL"
  printf 'LETSENCRYPT_EMAIL="%s"\n' "$LETSENCRYPT_EMAIL"
  printf 'GHCR_OWNER="%s"\n' "${GHCR_OWNER:-code-brew-git-hub}"
  printf 'FINTRACKER_VERSION="%s"\n' "${FINTRACKER_VERSION:-latest}"
} > .env

echo ">> .env created for server deploy (${DOMAIN})"
