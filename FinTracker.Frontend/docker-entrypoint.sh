#!/bin/sh
set -e

API_URL_VALUE="${API_URL:-http://localhost:5009/api}"

cat > /usr/share/nginx/html/js/api-config.js <<EOF
const API_URL = "${API_URL_VALUE}";
EOF

exec nginx -g 'daemon off;'
