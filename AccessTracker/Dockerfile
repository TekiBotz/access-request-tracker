# Build stage: use the full .NET SDK image to compile and publish the app.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore AccessTracker/AccessTracker.csproj
RUN dotnet publish AccessTracker/AccessTracker.csproj -c Release -o /app

# Run stage: a smaller runtime-only image that just runs the published app.
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "AccessTracker.dll"]