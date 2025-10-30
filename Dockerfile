# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln .
COPY RecipeParserV2.Api/*.csproj ./RecipeParserV2.Api/
COPY RecipeParserV2.Service/*.csproj ./RecipeParserV2.Service/
COPY RecipeParserV2.DAL/*.csproj ./RecipeParserV2.DAL/
COPY RecipeParserV2.Models/*.csproj ./RecipeParserV2.Models/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build and publish
RUN dotnet publish RecipeParserV2.Api/RecipeParserV2.Api.csproj -c Release -o /app/publish

# Use the official .NET 8 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Expose port 8080 (Render's default)
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "RecipeParserV2.Api.dll"]