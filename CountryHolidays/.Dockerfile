FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./ ./
RUN dotnet publish -c Release -o out

# Create table for cache in db
RUN /opt/mssql-tools/bin/sqlcmd create database DistCache go
RUN dotnet sql-cache create "Data Source=localhost;Initial Catalog=DistCache;Persist Security Info=True;User ID=sa;Password=AdminPassword1!" dbo CacheStore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "CountryHolidays.dll"]