# This stage is used when running from VS in fast mode
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

RUN apt-get update \
    && apt-get install -y --no-install-recommends libkrb5-3 \
    && rm -rf /var/lib/apt/lists/*

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["OnlineBookStore.Api/OnlineBookStore.Api.csproj", "OnlineBookStore.Api/"]
RUN dotnet restore "./OnlineBookStore.Api/OnlineBookStore.Api.csproj"

COPY . .
WORKDIR "/src/OnlineBookStore.Api"
RUN dotnet build "./OnlineBookStore.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OnlineBookStore.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

RUN mkdir -p /app/Logs

USER $APP_UID

ENTRYPOINT ["dotnet", "OnlineBookStore.Api.dll"]