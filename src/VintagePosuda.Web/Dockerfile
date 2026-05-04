# =====================================================================
# Multi-stage Dockerfile для VintagePosuda.Web (Blazor Server, .NET 8).
#
# Стадия 1 (sdk):   восстанавливает зависимости и публикует приложение
#                   (отдельный слой для restore ускоряет повторные сборки).
# Стадия 2 (aspnet): минимальный runtime-образ; запускает как non-root
#                   пользователь app, добавляет curl для healthcheck.
# =====================================================================

# ------------------------- Стадия 1: BUILD ---------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Сначала копируем только csproj — кэшируется слой restore.
COPY src/VintagePosuda.Web/VintagePosuda.Web.csproj src/VintagePosuda.Web/
RUN dotnet restore src/VintagePosuda.Web/VintagePosuda.Web.csproj

# Теперь — весь исходный код.
COPY src/ src/

# Публикация в Release.
RUN dotnet publish src/VintagePosuda.Web/VintagePosuda.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ------------------------ Стадия 2: RUNTIME --------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# curl нужен для HEALTHCHECK в docker-compose.yml.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Создаём каталог для SQLite-базы и отдаём его non-root пользователю app
# (он уже создан в базовом образе с UID 1654).
RUN mkdir -p /app/data && chown -R app:app /app

USER app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build --chown=app:app /app/publish ./

ENTRYPOINT ["dotnet", "VintagePosuda.Web.dll"]
