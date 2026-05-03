FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copiamos los csproj primero para aprovechar el cache de capas
COPY StayWize.Domain/StayWize.Domain.csproj           StayWize.Domain/
COPY StayWize.Application/StayWize.Application.csproj StayWize.Application/
COPY StayWize.Infrastructure/StayWize.Infrastructure.csproj StayWize.Infrastructure/
COPY StayWize.Services/StayWize.Services.csproj       StayWize.Services/
COPY StayWize.API/StayWize.API.csproj                 StayWize.API/

# Restauramos dependencias
RUN dotnet restore StayWize.API/StayWize.API.csproj

# Copiamos todo el código fuente
COPY . .

# Compilamos y publicamos
RUN dotnet restore StayWize.API/StayWize.API.csproj

RUN dotnet publish StayWize.API/StayWize.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "StayWize.API.dll"]