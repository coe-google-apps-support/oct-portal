#!/bin/bash
#parse out the domain name of OCTAVA_URL
DOMAIN_NAME="$(echo $OCTAVA_URL | awk -F'/' '{print $3}')"

#remove all quotes (if any)
DOMAIN_NAME=$(echo "$DOMAIN_NAME" | tr -d '"')

REGEX=$(printf "s/%s/%s/g" '\${HOST_NAME}' "$DOMAIN_NAME")

/bin/sed -i $REGEX /etc/nginx/conf.d/default.conf