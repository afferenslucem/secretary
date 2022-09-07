FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet build "secretary.documents.tests" -c Release
COPY tests/config.json /secretary.documents.tests/bin/Release/net6.0/config.json
COPY tests/config.json /secretary.telegram.tests/bin/Release/net6.0/config.json
ENTRYPOINT ["dotnet", "test"]