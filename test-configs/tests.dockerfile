FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet build "Secretary.Documents.Tests" -c Release
COPY tests/config.json /Secretary.Documents.Tests/bin/Release/net6.0/config.json
COPY tests/config.json /Secretary.Telegram.Tests/bin/Release/net6.0/config.json
COPY tests/config.json /Secretary.Yandex.Tests/bin/Release/net6.0/config.json
ENTRYPOINT ["dotnet", "test"]