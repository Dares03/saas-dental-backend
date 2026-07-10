# ============================================================
#  Dockerfile  —  Backend .NET 10
#  Ubicación: saas-dental-backend/Dockerfile
#
#  Build multi-etapa: compila y publica solo lo necesario
#  Imagen final es pequeña (basada en runtime, no en SDK)
# ============================================================

# ── Etapa 1: Build ──────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar archivos de proyecto primero (para cachear restore)
COPY ["SaasDental.Api/SaasDental.Api.csproj", "SaasDental.Api/"]
COPY ["SaasDental.Application/SaasDental.Application.csproj", "SaasDental.Application/"]
COPY ["SaasDental.Domain/SaasDental.Domain.csproj", "SaasDental.Domain/"]
COPY ["SaasDental.Infrastructure/SaasDental.Infrastructure.csproj", "SaasDental.Infrastructure/"]

# Restaurar dependencias NuGet
RUN dotnet restore "SaasDental.Api/SaasDental.Api.csproj"

# Copiar el resto del código fuente
COPY . .

# Publicar en modo Release
WORKDIR /src/SaasDental.Api
RUN dotnet publish "SaasDental.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Etapa 2: Runtime (imagen final ligera) ───────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Usuario no-root por seguridad
RUN adduser --disabled-password --gecos '' appuser
USER appuser

COPY --from=build /app/publish .

# Puerto que expone la API (configurado en docker-compose)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SaasDental.Api.dll"]
