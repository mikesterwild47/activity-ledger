#!/usr/bin/env bash
# Emits N sign-in events for a tenant, then reports how many were stored.
set -euo pipefail

BASE_URL="${BASE_URL:-http://localhost:5080}"
TENANT="${1:-33333333-3333-3333-3333-333333333333}"
COUNT="${2:-50}"

for i in $(seq 1 "$COUNT"); do
  curl -s -o /dev/null -X POST "$BASE_URL/tenants/$TENANT/events" \
    -H "Content-Type: application/json" \
    -d "{\"messageId\":\"batch-$i\",\"eventType\":\"user.signed_in\",\"actor\":\"user$i@example.test\"}"
done

sleep 2
stored=$(curl -s "$BASE_URL/tenants/$TENANT/events?pageSize=1" | grep -o '"totalCount":[0-9]*' | cut -d: -f2)
echo "Sent $COUNT events, stored $stored."
