FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["TournamentPlatformSystemWebApi.API/TournamentPlatformSystemWebApi.API.csproj", "TournamentPlatformSystemWebApi.API/"]
COPY ["TournamentPlatformSystemWebApi.Application/TournamentPlatformSystemWebApi.Application.csproj", "TournamentPlatformSystemWebApi.Application/"]
COPY ["TournamentPlatformSystemWebApi.Core/TournamentPlatformSystemWebApi.Core.csproj", "TournamentPlatformSystemWebApi.Core/"]
COPY ["TournamentPlatformSystemWebApi.Infrastructure/TournamentPlatformSystemWebApi.Infrastructure.csproj", "TournamentPlatformSystemWebApi.Infrastructure/"]
COPY ["TournamentPlatformSystemWebApi.Common/TournamentPlatformSystemWebApi.Common.csproj", "TournamentPlatformSystemWebApi.Common/"]

RUN dotnet restore "TournamentPlatformSystemWebApi.API/TournamentPlatformSystemWebApi.API.csproj"

# Copy remaining source and publish
COPY . .
WORKDIR /src/TournamentPlatformSystemWebApi.API
RUN dotnet publish -c Debug -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

RUN dotnet tool install --global dotnet-ef --version 9.0.15
ENV PATH="$PATH:/root/.dotnet/tools"


COPY --from=build /src /src





# Let Kestrel listen on port 80
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
COPY seeds/seed.sql .
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]
