FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY ideas.sln ./
COPY ideas-server/ideas-server/ideas-server.csproj ideas-server/ideas-server/
COPY C:\Users\Dan\Documents\GitHub\oct-portal\server\ideas-server\ideas-server\nuget.config ideas-server/ideas-server/
COPY ideas-core/ideas-core/ideas-core.csproj ideas-core/ideas-core/
COPY C:\Users\Dan\Documents\GitHub\oct-portal\server\ideas-core\ideas-core\nuget.config ideas-core/ideas-core/
COPY shared-kernel/shared-kernel.csproj shared-kernel/
COPY C:\Users\Dan\Documents\GitHub\oct-portal\server\shared-kernel\nuget.config shared-kernel/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/ideas-server/ideas-server
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ideas-server.dll"]
