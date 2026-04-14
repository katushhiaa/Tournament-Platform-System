FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["TournamentPlatformSystemWebApi.API/TournamentPlatformSystemWebApi.API.csproj", "TournamentPlatformSystemWebApi.API/"]
COPY ["TournamentPlatformSystem.Application/TournamentPlatformSystemWebApi.Application.csproj", "TournamentPlatformSystem.Application/"]
COPY ["TournamentPlatformSystemWebApi.Core/TournamentPlatformSystemWebApi.Core.csproj", "TournamentPlatformSystemWebApi.Core/"]
COPY ["TournamentPlatformSystemWebApi.Infrastructure/TournamentPlatformSystemWebApi.Infrastructure.csproj", "TournamentPlatformSystemWebApi.Infrastructure/"]
COPY ["TournamentPlatformSystemWebApi.Common/TournamentPlatformSystemWebApi.Common.csproj", "TournamentPlatformSystemWebApi.Common/"]

RUN dotnet restore "TournamentPlatformSystemWebApi.API/TournamentPlatformSystemWebApi.API.csproj"

# Copy remaining source and publish
COPY . .
WORKDIR /src/TournamentPlatformSystemWebApi.API
RUN dotnet publish -c Debug -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Let Kestrel listen on port 80
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TournamentPlatformSystemWebApi.API.dll"]
