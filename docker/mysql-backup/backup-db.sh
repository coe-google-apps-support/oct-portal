mysqldump -u $DB_USER -p$DB_PASSWORD -P $DB_PORT -h $DB_HOST $DB_NAME > /latest.sql
azcopy --source /latest.sql --destination https://octavia.blob.core.windows.net/dbbackup/OctPortalWordPress_latest.sql --dest-key $AZ_COPY_KEY --quiet
