FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY ideas-integration-remedy/ideas-integration-remedy-watcher/ideas-integration-remedy-watcher.csproj ideas-integration-remedy/ideas-integration-remedy-watcher/
COPY C:\Users\CoopDev\Desktop\oct-portal\server\ideas-integration-remedy\ideas-integration-remedy-watcher\nuget.config ideas-integration-remedy/ideas-integration-remedy-watcher/
COPY ideas-core/ideas-core/ideas-core.csproj ideas-core/ideas-core/
COPY C:\Users\CoopDev\Desktop\oct-portal\server\ideas-core\ideas-core\nuget.config ideas-core/ideas-core/
COPY shared-kernel/shared-kernel.csproj shared-kernel/
COPY C:\Users\CoopDev\Desktop\oct-portal\server\shared-kernel\nuget.config shared-kernel/
RUN dotnet restore ideas-integration-remedy/ideas-integration-remedy-watcher/ideas-integration-remedy-watcher.csproj
COPY . .
WORKDIR /src/ideas-integration-remedy/ideas-integration-remedy-watcher
RUN dotnet build ideas-integration-remedy-watcher.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ideas-integration-remedy-watcher.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ideas-integration-remedy-watcher.dll"]
