FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS base
WORKDIR /app


FROM base AS build
WORKDIR /src
COPY ["secretary.console/secretary.console.csproj", "secretary.console/secretary.console.csproj"]
RUN dotnet restore "secretary.console/secretary.console.csproj"
COPY . .
WORKDIR /src/secretary.console
RUN dotnet build "secretary.console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "secretary.console.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0-bullseye-slim AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "secretary.console.dll"]