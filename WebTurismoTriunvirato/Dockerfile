# -------------------------------------------------------------------------
# ETAPA 1: BASE - Imagen mínima de ejecución de ASP.NET Core
# -------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# -------------------------------------------------------------------------
# ETAPA 2: BUILD - Compilación del código
# -------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiamos TODOS los archivos del repositorio a /src.
COPY . .

# Ejecutamos restore
RUN dotnet restore "WebTurismoTriunvirato.csproj"

# [CAMBIO CLAVE] Ejecutamos build con alta verbosidad (/v:d)
# Esto DEBE forzar a que el log muestre el error exacto de compilación (CSxxxx).
RUN dotnet build "WebTurismoTriunvirato.csproj" -c $BUILD_CONFIGURATION -o /app/build /v:d

# -------------------------------------------------------------------------
# ETAPA 3: PUBLISH
# -------------------------------------------------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebTurismoTriunvirato.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -------------------------------------------------------------------------
# ETAPA 4: FINAL - Imagen de ejecución (RUNTIME)
# -------------------------------------------------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# === CORRECCIÓN DE PERMISOS DE ESCRITURA (para el error 405) ===
# Se ejecuta como ROOT.
RUN chown -R app:app wwwroot/img/promocionesvuelos
RUN chmod -R 775 wwwroot/img/promocionesvuelos

# Definimos el usuario 'app' para la ejecución final.
USER app 

ENTRYPOINT ["dotnet", "WebTurismoTriunvirato.dll"]
