namespace RecipeParserV2.Models
{
    /// <summary>
    /// Response model for TheMealDB API calls
    /// </summary>
    public class TheMealDBResponse<T>
    {
        public List<T> Meals { get; set; } = new List<T>();
    }

    /// <summary>
    /// Meal model from TheMealDB
    /// </summary>
    public class Meal
    {
        public string IdMeal { get; set; } = string.Empty;
        public string StrMeal { get; set; } = string.Empty;
        public string StrMealThumb { get; set; } = string.Empty;
        public string StrCategory { get; set; } = string.Empty;
        public string StrArea { get; set; } = string.Empty;
        public string StrInstructions { get; set; } = string.Empty;
        public string StrTags { get; set; } = string.Empty;

        // Ingredients (up to 20)
        public string StrIngredient1 { get; set; } = string.Empty;
        public string StrIngredient2 { get; set; } = string.Empty;
        public string StrIngredient3 { get; set; } = string.Empty;
        public string StrIngredient4 { get; set; } = string.Empty;
        public string StrIngredient5 { get; set; } = string.Empty;
        public string StrIngredient6 { get; set; } = string.Empty;
        public string StrIngredient7 { get; set; } = string.Empty;
        public string StrIngredient8 { get; set; } = string.Empty;
        public string StrIngredient9 { get; set; } = string.Empty;
        public string StrIngredient10 { get; set; } = string.Empty;

        // Measurements (up to 20)
        public string StrMeasure1 { get; set; } = string.Empty;
        public string StrMeasure2 { get; set; } = string.Empty;
        public string StrMeasure3 { get; set; } = string.Empty;
        public string StrMeasure4 { get; set; } = string.Empty;
        public string StrMeasure5 { get; set; } = string.Empty;
        public string StrMeasure6 { get; set; } = string.Empty;
        public string StrMeasure7 { get; set; } = string.Empty;
        public string StrMeasure8 { get; set; } = string.Empty;
        public string StrMeasure9 { get; set; } = string.Empty;
        public string StrMeasure10 { get; set; } = string.Empty;

        /// <summary>
        /// Get all non-empty ingredients
        /// </summary>
        public List<string> GetIngredients()
        {
            var ingredients = new List<string>();
            var ingredientProperties = new[]
            {
                StrIngredient1, StrIngredient2, StrIngredient3, StrIngredient4, StrIngredient5,
                StrIngredient6, StrIngredient7, StrIngredient8, StrIngredient9, StrIngredient10
            };

            foreach (var ingredient in ingredientProperties)
            {
                if (!string.IsNullOrWhiteSpace(ingredient))
                {
                    ingredients.Add(ingredient.Trim());
                }
            }

            return ingredients;
        }
    }

    /// <summary>
    /// Category model from TheMealDB
    /// </summary>
    public class Category
    {
        public string StrCategory { get; set; } = string.Empty;
    }

    /// <summary>
    /// Area (cuisine) model from TheMealDB
    /// </summary>
    public class Area
    {
        public string StrArea { get; set; } = string.Empty;
    }

    /// <summary>
    /// Ingredient model from TheMealDB
    /// </summary>
    public class Ingredient
    {
        public string IdIngredient { get; set; } = string.Empty;
        public string StrIngredient { get; set; } = string.Empty;
        public string StrDescription { get; set; } = string.Empty;
        public string StrType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Simplified recipe model for our application
    /// </summary>
    public class Recipe
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public List<string> Ingredients { get; set; } = new List<string>();
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// Convert from TheMealDB Meal model
        /// </summary>
        public static Recipe FromMeal(Meal meal)
        {
            var tags = new List<string>();
            if (!string.IsNullOrWhiteSpace(meal.StrTags))
            {
                tags = meal.StrTags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim().ToLowerInvariant())
                    .ToList();
            }

            return new Recipe
            {
                Id = meal.IdMeal,
                Name = meal.StrMeal,
                Thumbnail = meal.StrMealThumb,
                Category = meal.StrCategory,
                Area = meal.StrArea,
                Instructions = meal.StrInstructions,
                Ingredients = meal.GetIngredients(),
                Tags = tags
            };
        }
    }

    /// <summary>
    /// Response model for dropdown options
    /// </summary>
    public class DropdownOptionsResponse
    {
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> Areas { get; set; } = new List<string>();
        public List<string> Ingredients { get; set; } = new List<string>();
    }
}