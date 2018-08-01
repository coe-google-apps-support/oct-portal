FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY ideas.sln ./
COPY ../ideas-integration-apex/ideas-integration-apex.csproj ../ideas-integration-apex/
COPY ideas-core/ideas-core/ideas-core.csproj ideas-core/ideas-core/
COPY C:\Users\fenluo\Documents\GitHub\oct-portal\server\ideas-core\ideas-core\nuget.config ideas-core/ideas-core/
COPY shared-kernel/shared-kernel.csproj shared-kernel/
COPY C:\Users\fenluo\Documents\GitHub\oct-portal\server\shared-kernel\nuget.config shared-kernel/
COPY ideas-server/ideas-server/ideas-server.csproj ideas-server/ideas-server/
COPY C:\Users\fenluo\Documents\GitHub\oct-portal\server\ideas-server\ideas-server\nuget.config ideas-server/ideas-server/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/../ideas-integration-apex
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ideas-integration-apex.dll"]
