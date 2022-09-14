FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS base
WORKDIR /app


FROM base AS build
WORKDIR /src
COPY ["Secretary.Console/Secretary.Console.csproj", "Secretary.Console/Secretary.Console.csproj"]
RUN dotnet restore "Secretary.Console/Secretary.Console.csproj"
COPY . .
WORKDIR /src/Secretary.Console
RUN dotnet build "Secretary.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Secretary.Console.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0-bullseye-slim AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Secretary.Console.dll"]