FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

EXPOSE 80
EXPOSE 443
# Copy everything
COPY . .
# Restore as distinct layers
RUN dotnet restore ./Foodcourt.Api/Foodcourt.Api.csproj
# Build and publish a release
COPY . .
RUN dotnet publish ./Foodcourt.Api/Foodcourt.Api.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Foodcourt.Api.dll"]