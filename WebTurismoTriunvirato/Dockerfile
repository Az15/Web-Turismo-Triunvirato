# Primera etapa: Base Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# NOTA: NO USAMOS USER app AQUÍ TODAVÍA, para permitir que el paso RUN funcione como root.
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

# === INICIO DE CORRECCIÓN DE PERMISOS (EJECUTADO COMO ROOT) ===
# 2. Cambiamos el propietario de la carpeta al usuario 'app'. Esto es vital.
RUN chown -R app:app wwwroot/img/promocionesvuelos

# 3. Asignamos permisos 775 (rwxrwxr-x) recursivamente.
RUN chmod -R 775 wwwroot/img/promocionesvuelos

# === FIN DE CORRECCIÓN DE PERMISOS ===

# Ahora, y solo ahora, cambiamos al usuario 'app' para la ejecución final.
USER app 

ENTRYPOINT ["dotnet", "WebTurismoTriunvirato.dll"]
