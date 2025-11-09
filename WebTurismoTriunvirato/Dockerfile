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

# [CORRECCIÓN CLAVE] Copiamos TODOS los archivos del repositorio a /src de una vez.
# Esto asegura que todos los archivos fuente (.cs) estén presentes para la compilación,
# resolviendo el 'exit code 1' en la compilación.
COPY . .

# Ejecutamos restore y build.
RUN dotnet restore "WebTurismoTriunvirato.csproj"
RUN dotnet build "WebTurismoTriunvirato.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -------------------------------------------------------------------------
# ETAPA 3: PUBLISH - Prepara los archivos finales
# -------------------------------------------------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebTurismoTriunvirato.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -------------------------------------------------------------------------
# ETAPA 4: FINAL - Imagen de ejecución de la aplicación (RUNTIME)
# -------------------------------------------------------------------------
FROM base AS final
WORKDIR /app
# Copia la aplicación publicada al entorno de ejecución
COPY --from=publish /app/publish .

# === CORRECCIÓN DE PERMISOS DE ESCRITURA (para solucionar el error 405) ===
# Ejecutado como ROOT. La carpeta 'wwwroot/img/promocionesvuelos' ya existe.

# 1. Cambiamos el propietario de la carpeta al usuario 'app'.
RUN chown -R app:app wwwroot/img/promocionesvuelos

# 2. Asignamos permisos 775 (rwxrwxr-x) recursivamente, dando escritura al usuario 'app'.
RUN chmod -R 775 wwwroot/img/promocionesvuelos

# 3. Definimos el usuario 'app' para la ejecución final (seguridad).
USER app 

ENTRYPOINT ["dotnet", "WebTurismoTriunvirato.dll"]

