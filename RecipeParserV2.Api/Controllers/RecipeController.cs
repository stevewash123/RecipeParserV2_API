using Microsoft.AspNetCore.Mvc;
using RecipeParserV2.Models;
using RecipeParserV2.Service.Interfaces;

namespace RecipeParserV2.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        /// <summary>
        /// Get dropdown options for the UI (first 5 of each type)
        /// </summary>
        [HttpGet("dropdown-options")]
        public async Task<ActionResult<DropdownOptionsResponse>> GetDropdownOptions()
        {
            try
            {
                var options = await _recipeService.GetDropdownOptionsAsync();
                return Ok(options);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to fetch dropdown options", details = ex.Message });
            }
        }

        /// <summary>
        /// Search recipes using Boolean query
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<RecipeSearchResponse>> SearchRecipes([FromBody] RecipeSearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Query))
            {
                return BadRequest(new { error = "Query is required" });
            }

            try
            {
                var response = await _recipeService.SearchRecipesAsync(request.Query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Search failed", details = ex.Message });
            }
        }

        /// <summary>
        /// Validate a Boolean query syntax
        /// </summary>
        [HttpGet("validate")]
        public ActionResult<QueryValidationResponse> ValidateQuery([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { error = "Query parameter is required" });
            }

            try
            {
                var response = _recipeService.ValidateQuery(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new QueryValidationResponse
                {
                    Query = query,
                    IsValid = false,
                    ErrorMessage = $"Validation error: {ex.Message}",
                    ParseTree = null
                });
            }
        }

        /// <summary>
        /// Get recipe by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipeById(string id)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeByIdAsync(id);
                if (recipe == null)
                {
                    return NotFound(new { error = $"Recipe with ID {id} not found" });
                }

                return Ok(recipe);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to fetch recipe", details = ex.Message });
            }
        }
    }
}