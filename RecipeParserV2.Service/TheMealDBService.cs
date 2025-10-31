using RecipeParserV2.Models;
using RecipeParserV2.Service.Interfaces;
using System.Text.Json;

namespace RecipeParserV2.Service
{
    /// <summary>
    /// Service for interacting with TheMealDB API
    /// </summary>
    public class TheMealDBService : ITheMealDBService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://www.themealdb.com/api/json/v1/1";

        public TheMealDBService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get dropdown options for the UI (first 5 of each category)
        /// </summary>
        public async Task<DropdownOptionsResponse> GetDropdownOptionsAsync()
        {
            var result = new DropdownOptionsResponse();

            try
            {
                // Get categories
                var categories = await GetCategoriesAsync();
                result.Categories = categories.Take(5).ToList();

                // Get areas (cuisines)
                var areas = await GetAreasAsync();
                result.Areas = areas.Take(5).ToList();

                // Get ingredients
                var ingredients = await GetIngredientsAsync();
                result.Ingredients = ingredients.Take(5).ToList();
            }
            catch (Exception ex)
            {
                // Log error but return partial results
                Console.WriteLine($"Error getting dropdown options: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Get all available categories
        /// </summary>
        public async Task<List<string>> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_baseUrl}/list.php?c=list");
                var result = JsonSerializer.Deserialize<TheMealDBResponse<Category>>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Meals?.Select(c => c.StrCategory).Where(c => !string.IsNullOrEmpty(c)).ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting categories: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Get all available areas (cuisines)
        /// </summary>
        public async Task<List<string>> GetAreasAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_baseUrl}/list.php?a=list");
                var result = JsonSerializer.Deserialize<TheMealDBResponse<Area>>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Meals?.Select(a => a.StrArea).Where(a => !string.IsNullOrEmpty(a)).ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting areas: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Get all available ingredients
        /// </summary>
        public async Task<List<string>> GetIngredientsAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_baseUrl}/list.php?i=list");
                var result = JsonSerializer.Deserialize<TheMealDBResponse<Ingredient>>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Meals?.Select(i => i.StrIngredient).Where(i => !string.IsNullOrEmpty(i)).ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting ingredients: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Search meals by category
        /// </summary>
        public async Task<List<Recipe>> GetMealsByCategoryAsync(string category)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_baseUrl}/filter.php?c={Uri.EscapeDataString(category)}");
                var result = JsonSerializer.Deserialize<TheMealDBResponse<Meal>>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Meals?.Select(Recipe.FromMeal).ToList() ?? new List<Recipe>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting meals by category {category}: {ex.Message}");
                return new List<Recipe>();
            }
        }

        /// <summary>
        /// Search meals by area (cuisine)
        /// </summary>
        public async Task<List<Recipe>> GetMealsByAreaAsync(string area)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_baseUrl}/filter.php?a={Uri.EscapeDataString(area)}");
                var result = JsonSerializer.Deserialize<TheMealDBResponse<Meal>>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Meals?.Select(Recipe.FromMeal).ToList() ?? new List<Recipe>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting meals by area {area}: {ex.Message}");
                return new List<Recipe>();
            }
        }

        /// <summary>
        /// Search meals by main ingredient
        /// </summary>
        public async Task<List<Recipe>> GetMealsByIngredientAsync(string ingredient)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_baseUrl}/filter.php?i={Uri.EscapeDataString(ingredient)}");
                var result = JsonSerializer.Deserialize<TheMealDBResponse<Meal>>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Meals?.Select(Recipe.FromMeal).ToList() ?? new List<Recipe>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting meals by ingredient {ingredient}: {ex.Message}");
                return new List<Recipe>();
            }
        }

        /// <summary>
        /// Get detailed meal information by ID
        /// </summary>
        public async Task<Recipe?> GetMealByIdAsync(string mealId)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_baseUrl}/lookup.php?i={Uri.EscapeDataString(mealId)}");
                var result = JsonSerializer.Deserialize<TheMealDBResponse<Meal>>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var meal = result?.Meals?.FirstOrDefault();
                return meal != null ? Recipe.FromMeal(meal) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting meal by ID {mealId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Search meals by name
        /// </summary>
        public async Task<List<Recipe>> SearchMealsByNameAsync(string name)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_baseUrl}/search.php?s={Uri.EscapeDataString(name)}");
                var result = JsonSerializer.Deserialize<TheMealDBResponse<Meal>>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Meals?.Select(Recipe.FromMeal).ToList() ?? new List<Recipe>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching meals by name {name}: {ex.Message}");
                return new List<Recipe>();
            }
        }
    }
}