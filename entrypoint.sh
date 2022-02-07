echo "running set up script"

/opt/mssql-tools/bin/sqlcmd -S localhost:1433 -U sa -P AdminPassword1! -i init-db.sql