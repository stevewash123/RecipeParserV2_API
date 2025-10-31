using RecipeParserV2.Models;

namespace RecipeParserV2.Service.Interfaces
{
    public interface ITheMealDBService
    {
        Task<DropdownOptionsResponse> GetDropdownOptionsAsync();
        Task<List<string>> GetCategoriesAsync();
        Task<List<string>> GetAreasAsync();
        Task<List<string>> GetIngredientsAsync();
        Task<List<Recipe>> GetMealsByCategoryAsync(string category);
        Task<List<Recipe>> GetMealsByAreaAsync(string area);
        Task<List<Recipe>> GetMealsByIngredientAsync(string ingredient);
        Task<Recipe?> GetMealByIdAsync(string mealId);
        Task<List<Recipe>> SearchMealsByNameAsync(string name);
    }
}