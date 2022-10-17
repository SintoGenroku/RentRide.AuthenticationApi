# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore "./Authentication.Api/Authentication.Api.csproj"
RUN dotnet publish -c Release -o out

# Serve stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Authentication.Api.dll"]