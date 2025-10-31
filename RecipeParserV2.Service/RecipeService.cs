using RecipeParserV2.Models;
using RecipeParserV2.Service.Interfaces;
using RecipeParserV2.Service.Parsers;
using System.Text.RegularExpressions;

namespace RecipeParserV2.Service
{
    public class RecipeService : IRecipeService
    {
        private readonly ITheMealDBService _mealService;

        public RecipeService(ITheMealDBService mealService)
        {
            _mealService = mealService;
        }

        public async Task<DropdownOptionsResponse> GetDropdownOptionsAsync()
        {
            return await _mealService.GetDropdownOptionsAsync();
        }

        public async Task<RecipeSearchResponse> SearchRecipesAsync(string query)
        {
            // Parse the Boolean query
            var parseResult = SimpleRecipeParser.ParseQuery(query);
            if (!parseResult.Success)
            {
                throw new InvalidOperationException($"Invalid query syntax: {parseResult.ErrorMessage}");
            }

            // For now, implement basic search - in a real implementation, you'd evaluate the AST
            var recipes = await SearchRecipesBySimpleTerms(query);

            var response = new RecipeSearchResponse
            {
                Query = query,
                Results = recipes,
                ParseTree = GenerateSimpleParseTree(parseResult.Value),
                ExecutionTime = "2ms", // Placeholder
                ResultCount = recipes.Count,
                GeneratedSQL = GenerateSQL(query)
            };

            return response;
        }

        public QueryValidationResponse ValidateQuery(string query)
        {
            var parseResult = SimpleRecipeParser.ParseQuery(query);

            var response = new QueryValidationResponse
            {
                Query = query,
                IsValid = parseResult.Success,
                ErrorMessage = parseResult.ErrorMessage,
                ParseTree = parseResult.Success ? GenerateSimpleParseTree(parseResult.Value) : null
            };

            return response;
        }

        public async Task<Recipe?> GetRecipeByIdAsync(string id)
        {
            return await _mealService.GetMealByIdAsync(id);
        }

        /// <summary>
        /// Simple search implementation (placeholder for full Boolean evaluation)
        /// </summary>
        private async Task<List<Recipe>> SearchRecipesBySimpleTerms(string query)
        {
            var allRecipes = new List<Recipe>();
            var searchTerms = ExtractSearchTerms(query);

            // Search by categories, areas, and ingredients
            foreach (var term in searchTerms)
            {
                // Try searching as category
                var categoryRecipes = await _mealService.GetMealsByCategoryAsync(term);
                allRecipes.AddRange(categoryRecipes);

                // Try searching as area
                var areaRecipes = await _mealService.GetMealsByAreaAsync(term);
                allRecipes.AddRange(areaRecipes);

                // Try searching as ingredient
                var ingredientRecipes = await _mealService.GetMealsByIngredientAsync(term);
                allRecipes.AddRange(ingredientRecipes);

                // Try searching by name
                var nameRecipes = await _mealService.SearchMealsByNameAsync(term);
                allRecipes.AddRange(nameRecipes);
            }

            // Remove duplicates and limit results
            return allRecipes
                .GroupBy(r => r.Id)
                .Select(g => g.First())
                .Take(20)
                .ToList();
        }

        /// <summary>
        /// Extract search terms from query (simplified)
        /// </summary>
        private List<string> ExtractSearchTerms(string query)
        {
            // Remove Boolean operators and parentheses for simple search
            var terms = query
                .Replace("AND", " ")
                .Replace("OR", " ")
                .Replace("NOT", " ")
                .Replace("(", " ")
                .Replace(")", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(term => term.Length > 2)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return terms;
        }

        /// <summary>
        /// Generate simple parse tree visualization
        /// </summary>
        private object? GenerateSimpleParseTree(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;

            var operators = new List<string>();
            var terms = new List<string>();

            // Extract all operators including NOT, (, and )
            var operatorPattern = @"\b(AND|OR|NOT)\b|\(|\)";
            var operatorMatches = Regex.Matches(query, operatorPattern, RegexOptions.IgnoreCase);

            foreach (Match match in operatorMatches)
            {
                operators.Add(match.Value.ToUpper());
            }

            // Extract terms by removing all operators and splitting on whitespace
            var termsText = query;

            // Remove operators while preserving word boundaries
            termsText = Regex.Replace(termsText, @"\b(AND|OR|NOT)\b", " ", RegexOptions.IgnoreCase);
            termsText = Regex.Replace(termsText, @"[()]", " ");

            // Split on whitespace and filter out empty strings
            var extractedTerms = termsText
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(term => term.Trim())
                .Where(term => !string.IsNullOrEmpty(term))
                .ToList();

            terms.AddRange(extractedTerms);

            return new
            {
                Type = "BooleanQuery",
                Query = query,
                Terms = terms.ToArray(),
                Operators = operators.ToArray(),
                HasParentheses = query.Contains("(") || query.Contains(")"),
                HasNot = query.Contains("NOT")
            };
        }

        /// <summary>
        /// Generate SQL query from Boolean expression
        /// </summary>
        private string GenerateSQL(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return string.Empty;

            var sql = @"SELECT r.*, c.CategoryName, a.AreaName
FROM Recipes r
LEFT JOIN Categories c ON r.CategoryId = c.Id
LEFT JOIN Areas a ON r.AreaId = a.Id
LEFT JOIN RecipeIngredients ri ON r.Id = ri.RecipeId
LEFT JOIN Ingredients i ON ri.IngredientId = i.Id
WHERE ";

            // Transform Boolean logic to SQL
            var whereClause = ConvertBooleanToSQL(query);
            sql += whereClause;
            sql += "\nGROUP BY r.Id\nORDER BY r.Name;";

            return sql;
        }

        /// <summary>
        /// Convert Boolean query to SQL WHERE clause
        /// </summary>
        private string ConvertBooleanToSQL(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return "1=1";

            var sqlWhere = query;

            // Handle cuisines/areas
            sqlWhere = Regex.Replace(sqlWhere,
                @"\b(american|british|canadian|chinese|croatian|dutch|egyptian|filipino|french|greek|indian|irish|italian|jamaican|japanese|kenyan|malaysian|mexican|moroccan|polish|portuguese|russian|spanish|thai|tunisian|turkish|ukrainian|vietnamese)\b",
                "a.AreaName = '$1'",
                RegexOptions.IgnoreCase);

            // Handle ingredients
            sqlWhere = Regex.Replace(sqlWhere,
                @"\b(chicken|salmon|beef|pork|avocado|lime|rice|onions|garlic|tomatoes|potatoes|carrots|mushrooms|peppers|cheese|butter)\b",
                "i.IngredientName LIKE '%$1%'",
                RegexOptions.IgnoreCase);

            // Handle categories
            sqlWhere = Regex.Replace(sqlWhere,
                @"\b(breakfast|dessert|goat|lamb|miscellaneous|pasta|seafood|side|starter|vegan|vegetarian)\b",
                "c.CategoryName = '$1'",
                RegexOptions.IgnoreCase);

            // Handle dietary restrictions and attributes
            sqlWhere = Regex.Replace(sqlWhere, @"\bvegetarian\b", "r.IsVegetarian = 1", RegexOptions.IgnoreCase);
            sqlWhere = Regex.Replace(sqlWhere, @"\bvegan\b", "r.IsVegan = 1", RegexOptions.IgnoreCase);
            sqlWhere = Regex.Replace(sqlWhere, @"\bgluten_free\b", "r.IsGlutenFree = 1", RegexOptions.IgnoreCase);
            sqlWhere = Regex.Replace(sqlWhere, @"\bquick\b", "r.CookTimeMinutes <= 30", RegexOptions.IgnoreCase);
            sqlWhere = Regex.Replace(sqlWhere, @"\beasy\b", "r.DifficultyLevel = 'Easy'", RegexOptions.IgnoreCase);

            // Handle NOT operator
            sqlWhere = Regex.Replace(sqlWhere, @"NOT\s+\(", "NOT (", RegexOptions.IgnoreCase);
            sqlWhere = Regex.Replace(sqlWhere, @"NOT\s+([^(]\S+)", "NOT $1", RegexOptions.IgnoreCase);

            return sqlWhere;
        }
    }
}