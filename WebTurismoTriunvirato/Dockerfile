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

# 1. Aseguramos la existencia de las carpetas de subida que podrían faltar
# El flag '-p' es crucial: crea la estructura de directorios y no da error si ya existe.
RUN mkdir -p wwwroot/img
RUN mkdir -p wwwroot/images

# 2. Cambiamos el propietario al usuario 'app'.
# Esto es necesario para que la aplicación pueda escribir.
RUN chown -R app:app wwwroot/img
RUN chown -R app:app wwwroot/images

# 3. Asignamos permisos 775 (rwxrwxr-x).
RUN chmod -R 775 wwwroot/img
RUN chmod -R 775 wwwroot/images


# === FIN DE CORRECCIÓN DE PERMISOS ===

ENTRYPOINT ["dotnet", "WebTurismoTriunvirato.dll"]
