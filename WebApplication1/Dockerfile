FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env
WORKDIR /app

# Copy and restore project dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy and build the application
COPY . ./
RUN dotnet publish -c Release -o /app

FROM gcr.io/google-appengine/aspnetcore:3.1
WORKDIR /app
COPY --from=build-env /app/ .

ENTRYPOINT ["dotnet", "PDNCOAH_API.dll"]