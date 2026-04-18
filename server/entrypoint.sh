#!/bin/sh
set -e
echo "Starting entrypoint script..."
# Check if this is the init container
if [ "$(hostname)" = "init" ]; then
    echo " Starting database initialization..."
    
    echo " Running migrations..."
    cd /src/TournamentPlatformSystemWebApi.Infrastructure
    dotnet ef database update -s ../TournamentPlatformSystemWebApi.API
    echo " Migrations completed"
    
    echo " Seeding database..."
    PGPASSWORD="${POSTGRES_PASSWORD}" psql -h db -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -f /app/seed.sql
    echo " Database seeding completed!"
    
    echo " Initialization finished successfully"
    exit 0
fi

# Otherwise run the API
exec dotnet TournamentPlatformSystemWebApi.API.dll