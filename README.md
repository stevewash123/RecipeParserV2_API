# RecipeParserV2 API

## Overview
Boolean query parser demonstration backend with N-tier architecture for portfolio showcase.

## Architecture
- **Backend**: C# ASP.NET Core Web API with N-tier structure
- **Parser**: Boolean query processing with AST generation
- **API Integration**: TheMealDB for recipe data

## Project Structure
```
RecipeParserV2_API/
├── RecipeParserV2.Api/        # Web API layer
├── RecipeParserV2.Service/    # Business logic layer
├── RecipeParserV2.DAL/        # Data access layer
├── RecipeParserV2.Models/     # Data models and DTOs
└── RecipeParserV2.sln         # Solution file
```

## Development
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run API
cd RecipeParserV2.Api
dotnet run
```

## Deployment
This API is deployed to Render.com as part of the RecipeParserV2 portfolio demonstration.

## Related Projects
- **UI Repository**: [RecipeParserV2_UI](https://github.com/stevewash123/RecipeParserV2_UI)
- **Project Portfolio**: [Projects Overview](https://github.com/stevewash123/Projects)