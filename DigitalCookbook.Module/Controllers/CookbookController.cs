using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Users;
using OrchardCore.Users.Models;
using YesSql;

namespace DigitalCookbook.Module.Controllers
{
    public class CookbookController : Controller
    {
        private readonly ISession _session;
        private readonly UserManager<IUser> _userManager;

        public CookbookController(
            ISession session,
            UserManager<IUser> userManager)
        {
            _session = session;
            _userManager = userManager;
        }

        [Route("")]
        [Route("recipes")]
        public async Task<IActionResult> Index(string allergen = null, string search = null)
        {
            var allRecipes = await _session
                .Query<ContentItem, ContentItemIndex>(x => x.ContentType == "Recipe" && x.Published)
                .ListAsync();

            var filteredRecipes = new List<ContentItem>();

            // First filter by allergen
            if (string.IsNullOrEmpty(allergen))
            {
                filteredRecipes = allRecipes.ToList();
            }
            else
            {
                foreach (var recipe in allRecipes)
                {
                    // Use dynamic with proper null checking
                    dynamic recipeContent = recipe.Content;
                    dynamic recipePart = recipeContent.Recipe;

                    string[] ingredientIds = Array.Empty<string>();
                    try
                    {
                        var ingredientsList = recipePart.IngredientsList;
                        if (ingredientsList != null)
                        {
                            var ids = ingredientsList.ContentItemIds;
                            if (ids != null)
                            {
                                ingredientIds = (ids as string[]) ?? Array.Empty<string>();
                            }
                        }
                    }
                    catch
                    {
                        ingredientIds = Array.Empty<string>();
                    }

                    bool containsAllergen = false;

                    foreach (var ingredientId in ingredientIds)
                    {
                        var ingredient = await _session.Query<ContentItem, ContentItemIndex>()
                            .Where(x => x.ContentItemId == ingredientId)
                            .FirstOrDefaultAsync();

                        if (ingredient != null)
                        {
                            try
                            {
                                dynamic ingredientContent = ingredient.Content;
                                dynamic nutritionPart = ingredientContent.NutritionPart;
                                if (nutritionPart != null)
                                {
                                    string allergenInfo = nutritionPart.AllergenInfo?.ToString();
                                    if (!string.IsNullOrEmpty(allergenInfo) && allergenInfo != "None" && allergenInfo == allergen)
                                    {
                                        containsAllergen = true;
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                // Ignore errors for this ingredient
                            }
                        }
                    }

                    if (!containsAllergen)
                    {
                        filteredRecipes.Add(recipe);
                    }
                }
            }

            // Then filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                var searchResults = new List<ContentItem>();
                foreach (var recipe in filteredRecipes)
                {
                    // Check recipe name
                    bool matchesSearch = false;

                    if (recipe.DisplayText != null && recipe.DisplayText.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        matchesSearch = true;
                    }

                    // Check ingredients if not already matched
                    if (!matchesSearch)
                    {
                        try
                        {
                            dynamic recipeContent = recipe.Content;
                            dynamic recipePart = recipeContent.Recipe;
                            var ingredientsList = recipePart.IngredientsList;

                            if (ingredientsList != null)
                            {
                                var ingredientIds = ingredientsList.ContentItemIds as string[] ?? Array.Empty<string>();
                                foreach (var ingredientId in ingredientIds)
                                {
                                    var ingredient = await _session.Query<ContentItem, ContentItemIndex>()
                                        .Where(x => x.ContentItemId == ingredientId)
                                        .FirstOrDefaultAsync();

                                    if (ingredient != null && ingredient.DisplayText != null &&
                                        ingredient.DisplayText.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        matchesSearch = true;
                                        break;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Ignore errors
                        }
                    }

                    if (matchesSearch)
                    {
                        searchResults.Add(recipe);
                    }
                }
                filteredRecipes = searchResults;
            }

            return View("CookbookGrid", filteredRecipes);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Route("recipes/favorite/{contentItemId}")]
        public async Task<IActionResult> ToggleFavorite(string contentItemId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var user = await _userManager.GetUserAsync(User) as User;
            if (user == null) return Unauthorized();

            var properties = user.Properties;
            var favoritesList = new List<string>();

            if (properties.TryGetPropertyValue("Favorites", out var favoritesNode) && favoritesNode != null)
            {
                if (favoritesNode["ItemIds"] is System.Text.Json.Nodes.JsonArray jsonArray)
                {
                    foreach (var item in jsonArray)
                    {
                        var id = item?.ToString();
                        if (!string.IsNullOrEmpty(id))
                        {
                            favoritesList.Add(id);
                        }
                    }
                }
            }

            bool isFavorite;
            if (favoritesList.Contains(contentItemId))
            {
                favoritesList.Remove(contentItemId);
                isFavorite = false;
            }
            else
            {
                favoritesList.Add(contentItemId);
                isFavorite = true;
            }

            var updatedArray = new System.Text.Json.Nodes.JsonArray();
            foreach (var id in favoritesList)
            {
                updatedArray.Add(id);
            }

            var updatedFavoritesObj = new System.Text.Json.Nodes.JsonObject
            {
                ["ItemIds"] = updatedArray
            };

            properties["Favorites"] = updatedFavoritesObj;
            await _userManager.UpdateAsync(user);

            // Return JSON so the frontend knows what message to show
            return Json(new { success = true, isFavorite = isFavorite });
        }

        [Authorize]
        [Route("recipes/favorites")]
        public async Task<IActionResult> MyFavorites()
        {
            var user = await _userManager.GetUserAsync(User) as User;
            var itemIds = new List<string>();

            if (user != null && user.Properties.TryGetPropertyValue("Favorites", out var favoritesNode) && favoritesNode != null)
            {
                if (favoritesNode["ItemIds"] is System.Text.Json.Nodes.JsonArray jsonArray)
                {
                    foreach (var item in jsonArray)
                    {
                        var id = item?.ToString();
                        if (!string.IsNullOrEmpty(id))
                        {
                            itemIds.Add(id);
                        }
                    }
                }
            }

            var allPublishedRecipes = await _session
                .Query<ContentItem, ContentItemIndex>(x => x.ContentType == "Recipe" && x.Published)
                .ListAsync();

            var favoriteRecipes = allPublishedRecipes.Where(x => itemIds.Contains(x.ContentItemId)).ToList();

            return View("CookbookGrid", favoriteRecipes);
        }
    }
}