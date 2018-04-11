#wait for the SQL Server to come up
sleep 30s

#run the setup script to create the DB and the schema in the DB
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P OctavaDev100! -d master -i sql-setup.sql