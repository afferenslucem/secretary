FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS base
WORKDIR /app


FROM base AS build
WORKDIR /src
COPY ["Secretary.LogTimeReminder/Secretary.LogTimeReminder.csproj", "Secretary.LogTimeReminder/Secretary.LogTimeReminder.csproj"]
RUN dotnet restore "Secretary.LogTimeReminder/Secretary.LogTimeReminder.csproj"
COPY . .
WORKDIR /src/Secretary.LogTimeReminder
RUN dotnet build "Secretary.LogTimeReminder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Secretary.LogTimeReminder.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0-bullseye-slim AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Secretary.LogTimeReminder.dll"]