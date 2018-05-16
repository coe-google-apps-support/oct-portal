#!/bin/bash
#parse out the domain name of OCTAVA_URL
DOMAIN_NAME="$(echo $OCTAVA_URL | awk -F'/' '{print $3}')"
REGEX=$(printf "s/%s/%s/g" '\${HOST_NAME}' "$DOMAIN_NAME")

/bin/sed -i $REGEX /etc/nginx/conf.d/default.conf