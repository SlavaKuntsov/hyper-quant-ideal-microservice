FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 61578

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["nuget.config", "."]
COPY ["nuget/", "/nuget/"]

COPY ["InfoSymbolServer/InfoSymbolServer.csproj", "InfoSymbolServer/"]
RUN dotnet restore "InfoSymbolServer/InfoSymbolServer.csproj"
COPY . .
WORKDIR "/src/InfoSymbolServer"
RUN dotnet build "InfoSymbolServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "InfoSymbolServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InfoSymbolServer.dll"]