FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# El .csproj está en la raíz, así que solo necesitas copiar el archivo directamente.
COPY ["Web Turismo Triunvirato.csproj", "."]
# El .csproj ahora se encuentra en el directorio raíz del contenedor.
RUN dotnet restore "Web Turismo Triunvirato.csproj"
COPY . .
# El WORKDIR también debe ser la raíz del proyecto.
WORKDIR "/src"
RUN dotnet build "Web Turismo Triunvirato.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Web Turismo Triunvirato.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web Turismo Triunvirato.dll"]
