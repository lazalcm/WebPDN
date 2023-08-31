FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copia el archivo de proyecto y restaura las dependencias
COPY *.csproj .
RUN dotnet restore

# Copia el resto del código fuente y realiza la publicación
COPY . .
RUN dotnet publish -c Release -o out

# Crear imagen de aplicación final
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

# Define el punto de entrada de la aplicación
ENTRYPOINT ["dotnet", "PDNCOAH_API.dll"]
