echo "First attempt connecting to MSSQL."
until /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P OctavaDev100! -d master -Q "select getdate()"
do
  sleep 10s
  echo "connecting to MSSQL failed. Trying again."
done

echo "running the setup script to create the DB and the schema in the DB"
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P OctavaDev100! -d master -i sql-setup.sql