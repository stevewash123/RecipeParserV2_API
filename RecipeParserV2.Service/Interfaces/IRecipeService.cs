using RecipeParserV2.Models;

namespace RecipeParserV2.Service.Interfaces
{
    public interface IRecipeService
    {
        Task<DropdownOptionsResponse> GetDropdownOptionsAsync();
        Task<RecipeSearchResponse> SearchRecipesAsync(string query);
        QueryValidationResponse ValidateQuery(string query);
        Task<Recipe?> GetRecipeByIdAsync(string id);
    }
}