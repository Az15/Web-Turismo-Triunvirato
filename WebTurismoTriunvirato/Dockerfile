FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# CORREGIDO: Usamos el nombre real del archivo (.csproj) con guiones bajos.
COPY ["WebTurismoTriunvirato.csproj", "."]
# CORREGIDO: Usamos el nombre del archivo con guiones bajos para el restore.
RUN dotnet restore "WebTurismoTriunvirato.csproj"
COPY . .
# CORREGIDO: Usamos el nombre del archivo con guiones bajos para el build.
RUN dotnet build "WebTurismoTriunvirato.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# CORREGIDO: Usamos el nombre del archivo con guiones bajos para el publish.
RUN dotnet publish "WebTurismoTriunvirato.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# CORREGIDO: Usamos el nombre del archivo base (DLL) con guiones bajos para el ENTRYPOINT.
ENTRYPOINT ["dotnet", "WebTurismoTriunvirato.dll"]
