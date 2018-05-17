#!/bin/bash

echo "Setting NGINX config variables"

#remove all quotes (if any)
DOMAIN_NAME="$(echo $OCTAVA_URL | tr -d '"')"

echo "Setting NGINX config variables -1"

#parse out the domain name of OCTAVA_URL
DOMAIN_NAME="$(echo $DOMAIN_NAME | awk -F'/' '{print $3}')"

echo "Setting NGINX config variables -2"

#get the port (if any)
OCTAVA_PORT="$(echo $DOMAIN_NAME | awk -F':' '{print $2}')"

echo "Setting NGINX config variables -3"

# if no port defined use 80
if [[ -z "${OCTAVA_PORT// }" ]] ; then OCTAVA_PORT=80; fi

echo "Setting NGINX config variables -4"

#now remove port from DOMAIN_NAME
DOMAIN_NAME="$(echo $DOMAIN_NAME | awk -F':' '{print $1}')"

echo "Setting NGINX config variables -5"

REGEX=$(printf "s/%s/%s/g" '\${HOST_NAME}' "$DOMAIN_NAME")
echo "Replacing HOST_NAME using regex: $REGEX"
/bin/sed -i $REGEX /etc/nginx/conf.d/default.conf

echo "Setting NGINX config variables -6"

REGEX2=$(printf "s/%s/%s/g" '\${HOST_PORT}' "$OCTAVA_PORT")
echo "Replacing HOST_PORT using regex: $REGEX2"
/bin/sed -i $REGEX2 /etc/nginx/conf.d/default.conf

echo "Setting NGINX config variables -7"

echo "Current state of config:"
cat /etc/nginx/conf.d/default.conf

echo "Setting NGINX config variables -8"