FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY issues-server/issues-server/issues-server.csproj issues-server/issues-server/
COPY issues-core/issues-core/issues-core.csproj issues-core/issues-core/
COPY shared-kernel/shared-kernel.csproj shared-kernel/
COPY C:\Users\Dan\Documents\GitHub\oct-portal\server\shared-kernel\nuget.config shared-kernel/
RUN dotnet restore issues-server/issues-server/issues-server.csproj
COPY . .
WORKDIR /src/issues-server/issues-server
RUN dotnet build issues-server.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish issues-server.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "issues-server.dll"]
