FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TestingAlphaDeploy.Web/TestingAlphaDeploy.Web.csproj", "TestingAlphaDeploy.Web/"]
COPY ["TestingAlphaDeploy.ServiceDefaults/TestingAlphaDeploy.ServiceDefaults.csproj", "TestingAlphaDeploy.ServiceDefaults/"]
RUN dotnet restore "./TestingAlphaDeploy.Web/TestingAlphaDeploy.Web.csproj"
COPY . .
WORKDIR "/src/TestingAlphaDeploy.Web"
RUN dotnet build "./TestingAlphaDeploy.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# WORKDIR is /src/TestingAlphaDeploy.Web inherited from build stage
RUN dotnet publish "./TestingAlphaDeploy.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestingAlphaDeploy.Web.dll"]