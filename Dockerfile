# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first for layer caching
COPY TournamentAppBackend.slnx ./
COPY TournamentAppBackend/TournamentAppBackend.csproj TournamentAppBackend/
RUN dotnet restore TournamentAppBackend/TournamentAppBackend.csproj

# Copy the rest of the source
COPY . .
WORKDIR /src/TournamentAppBackend
RUN dotnet publish TournamentAppBackend.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TournamentAppBackend.dll"]