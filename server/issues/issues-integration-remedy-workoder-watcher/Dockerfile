FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY issues-integration-remedy-workoder-watcher/issues-integration-remedy-workoder-watcher.csproj issues-integration-remedy-workoder-watcher/
COPY issues-core/issues-core/issues-core.csproj issues-core/issues-core/
COPY ../shared-kernel/shared-kernel.csproj ../shared-kernel/
COPY C:\Users\CoopDev\Desktop\oct-portal\server\shared-kernel\nuget.config ../shared-kernel/
COPY issues-integration-remedy-watcher/issues-integration-remedy-watcher.csproj issues-integration-remedy-watcher/
RUN dotnet restore issues-integration-remedy-workoder-watcher/issues-integration-remedy-workoder-watcher.csproj
COPY . .
WORKDIR /src/issues-integration-remedy-workoder-watcher
RUN dotnet build issues-integration-remedy-workoder-watcher.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish issues-integration-remedy-workoder-watcher.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "issues-integration-remedy-workoder-watcher.dll"]
