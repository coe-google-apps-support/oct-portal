FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY ideas.sln ./
COPY ideas-integration-webhooks/ideas-integration-webhooks.csproj ideas-integration-webhooks/
COPY ideas-core/ideas-core/ideas-core.csproj ideas-core/ideas-core/
COPY C:\Users\CoopDev\Desktop\oct-portal\server\ideas-core\ideas-core\nuget.config ideas-core/ideas-core/
COPY shared-kernel/shared-kernel.csproj shared-kernel/
COPY C:\Users\CoopDev\Desktop\oct-portal\server\shared-kernel\nuget.config shared-kernel/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/ideas-integration-webhooks
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ideas-integration-webhooks.dll"]
