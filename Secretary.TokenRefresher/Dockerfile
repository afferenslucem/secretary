FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS base
WORKDIR /app


FROM base AS build
WORKDIR /src
COPY ["Secretary.TokenRefresher/Secretary.TokenRefresher.csproj", "Secretary.TokenRefresher/Secretary.TokenRefresher.csproj"]
RUN dotnet restore "Secretary.TokenRefresher/Secretary.TokenRefresher.csproj"
COPY . .
WORKDIR /src/Secretary.TokenRefresher
RUN dotnet build "Secretary.TokenRefresher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Secretary.TokenRefresher.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0-bullseye-slim AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Secretary.TokenRefresher.dll"]