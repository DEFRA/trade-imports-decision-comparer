# Base dotnet image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Add curl to template.
# CDP PLATFORM HEALTHCHECK REQUIREMENT
RUN apt update && \
    apt upgrade -y && \
    apt install curl -y && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY .config/dotnet-tools.json .config/dotnet-tools.json
COPY .csharpierrc .csharpierrc
COPY .csharpierignore .csharpierignore

RUN dotnet tool restore

COPY src/Comparer/Comparer.csproj src/Comparer/Comparer.csproj
COPY tests/Testing/Testing.csproj tests/Testing/Testing.csproj
COPY tests/Comparer.Tests/Comparer.Tests.csproj tests/Comparer.Tests/Comparer.Tests.csproj
COPY tests/Comparer.IntegrationTests/Comparer.IntegrationTests.csproj tests/Comparer.IntegrationTests/Comparer.IntegrationTests.csproj
COPY Defra.TradeImportsDecisionComparer.sln Defra.TradeImportsDecisionComparer.sln
COPY Directory.Build.props Directory.Build.props

COPY NuGet.config NuGet.config
ARG DEFRA_NUGET_PAT

RUN dotnet restore

COPY src/Comparer src/Comparer
COPY tests/Testing tests/Testing
COPY tests/Comparer.Tests tests/Comparer.Tests
COPY tests/Comparer.IntegrationTests tests/Comparer.IntegrationTests

RUN dotnet csharpier check .

RUN dotnet build src/Comparer/Comparer.csproj --no-restore -c Release

RUN dotnet test --no-restore --filter "Category!=IntegrationTest"

FROM build AS publish

RUN dotnet publish src/Comparer -c Release -o /app/publish /p:UseAppHost=false

ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8085
ENTRYPOINT ["dotnet", "Defra.TradeImportsDecisionComparer.Comparer.dll"]
