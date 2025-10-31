using System.Text.RegularExpressions;

namespace RecipeParserV2.Service.Parsers
{
    /// <summary>
    /// Simple recipe query parser for Boolean expressions
    /// </summary>
    public static class SimpleRecipeParser
    {
        /// <summary>
        /// Parse a simple Boolean query and return a result
        /// </summary>
        public static ParseResult<string> ParseQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new ParseResult<string>(string.Empty, false, "Query cannot be empty");

            try
            {
                // Basic validation - check for balanced parentheses
                var openParens = query.Count(c => c == '(');
                var closeParens = query.Count(c => c == ')');

                if (openParens != closeParens)
                    return new ParseResult<string>(string.Empty, false, "Unbalanced parentheses");

                // Check for valid operators
                var validPattern = @"^[a-zA-Z_\s\(\)]+(?:\s+(AND|OR|NOT)\s+[a-zA-Z_\s\(\)]+)*$";
                if (!Regex.IsMatch(query.Trim(), validPattern, RegexOptions.IgnoreCase))
                    return new ParseResult<string>(string.Empty, false, "Invalid query syntax");

                return new ParseResult<string>(query.Trim(), true, null);
            }
            catch (Exception ex)
            {
                return new ParseResult<string>(string.Empty, false, $"Parse error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Simple parse result class
    /// </summary>
    public class ParseResult<T>
    {
        public ParseResult(T value, bool success, string? errorMessage)
        {
            Value = value;
            Success = success;
            ErrorMessage = errorMessage;
        }

        public T Value { get; }
        public bool Success { get; }
        public string? ErrorMessage { get; }
    }
}