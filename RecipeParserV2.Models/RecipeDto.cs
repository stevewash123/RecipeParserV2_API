namespace RecipeParserV2.Models
{
    /// <summary>
    /// Request model for recipe search
    /// </summary>
    public class RecipeSearchRequest
    {
        public string Query { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response model for recipe search
    /// </summary>
    public class RecipeSearchResponse
    {
        public string Query { get; set; } = string.Empty;
        public List<Recipe> Results { get; set; } = new List<Recipe>();
        public object? ParseTree { get; set; }
        public string ExecutionTime { get; set; } = string.Empty;
        public int ResultCount { get; set; }
        public string GeneratedSQL { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response model for query validation
    /// </summary>
    public class QueryValidationResponse
    {
        public string Query { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public object? ParseTree { get; set; }
    }
}