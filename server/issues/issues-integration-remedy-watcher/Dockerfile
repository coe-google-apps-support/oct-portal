FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY issues-integration-remedy-watcher/issues-integration-remedy-watcher.csproj issues-integration-remedy-watcher/
COPY issues-core/issues-core/issues-core.csproj issues-core/issues-core/
COPY ../shared-kernel/shared-kernel.csproj ../shared-kernel/
COPY C:\Users\CoopDev\Desktop\oct-portal\server\shared-kernel\nuget.config ../shared-kernel/
RUN dotnet restore issues-integration-remedy-watcher/issues-integration-remedy-watcher.csproj
COPY . .
WORKDIR /src/issues-integration-remedy-watcher
RUN dotnet build issues-integration-remedy-watcher.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish issues-integration-remedy-watcher.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "issues-integration-remedy-watcher.dll"]
