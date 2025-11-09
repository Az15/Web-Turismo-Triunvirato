FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Segunda etapa: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebTurismoTriunvirato.csproj", "."]
RUN dotnet restore "WebTurismoTriunvirato.csproj"
COPY . .
RUN dotnet build "WebTurismoTriunvirato.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Tercera etapa: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebTurismoTriunvirato.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Cuarta etapa: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# === INICIO DE CORRECCIÓN DE PERMISOS PARA SUBIDAS ===

# El usuario 'app' necesita permisos de escritura en esta carpeta.
# 1. Creamos la estructura de directorios si no existe.
#RUN mkdir -p wwwroot/img/promocionesvuelos

# 2. Cambiamos el propietario de la carpeta de subidas al usuario 'app'.
# Esto es crucial para que el proceso de la aplicación (que corre como 'app') pueda escribir.
RUN chown -R app:app wwwroot/img/promocionesvuelos

# 3. Asignamos permisos 775 (rwxrwxr-x) recursivamente a la carpeta de subidas.
# Esto asegura que el dueño ('app') tenga permiso de ESCRITURA.
RUN chmod -R 775 wwwroot/img/promocionesvuelos

# === FIN DE CORRECCIÓN DE PERMISOS ===

ENTRYPOINT ["dotnet", "WebTurismoTriunvirato.dll"]
