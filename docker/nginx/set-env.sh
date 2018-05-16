#!/bin/bash

#remove all quotes (if any)
DOMAIN_NAME=$(echo "$OCTAVA_URL" | tr -d '"')

#parse out the domain name of OCTAVA_URL
DOMAIN_NAME="$(echo $DOMAIN_NAME | awk -F'/' '{print $3}')"

#get the port (if any)
OCTAVA_PORT="$(echo $DOMAIN_NAME | awk -F':' '{print $2}')"

# if no port defined use 80
if [[ -z "${OCTAVA_PORT// }" ]] ; then OCTAVA_PORT=80; fi

#now remove port from DOMAIN_NAME
DOMAIN_NAME="$(echo $DOMAIN_NAME | awk -F':' '{print $1}')"

REGEX=$(printf "s/%s/%s/g" '\${HOST_NAME}' "$DOMAIN_NAME")
/bin/sed -i $REGEX /etc/nginx/conf.d/default.conf

REGEX2=$(printf "s/%s/%s/g" '\${HOST_PORT}' "$OCTAVA_PORT")
/bin/sed -i $REGEX2 /etc/nginx/conf.d/default.conf