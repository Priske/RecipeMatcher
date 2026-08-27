using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeMatcher.Web.Data;
using RecipeMatcher.Web.Models;

namespace RecipeMatcher.Web.Controllers;

public class MatcherController : Controller
{
    private readonly AppDbContext _dbContext;

    public MatcherController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var ingredients = await _dbContext.Ingredients
            .OrderBy(ingredient => ingredient.Name)
            .ToListAsync();

        var model = new MatcherViewModel
        {
            Ingredients = ingredients
                .Select(ingredient => new IngredientOptionViewModel
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    Selected = false
                })
                .ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(int[]? ingredientIds)
    {
        ingredientIds ??= [];

        if (ingredientIds.Length == 0)
        {
            return Content("No ingredient IDs selected.");
        }

        var ingredients = await _dbContext.Ingredients
            .OrderBy(ingredient => ingredient.Name)
            .ToListAsync();

        var model = new MatcherViewModel
        {
            Ingredients = ingredients
                .Select(ingredient => new IngredientOptionViewModel
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    Selected = ingredientIds.Contains(ingredient.Id)
                })
                .ToList(),

            Recipes = await _dbContext.Recipes
                .Where(recipe => recipe.RecipeIngredients
                    .Any(ri => ingredientIds.Contains(ri.IngredientId)))
                .Select(recipe => new RecipeResultViewModel
                {
                    RecipeId = recipe.Id,
                    Name = recipe.Name,
                    PreparationMinutes = recipe.PreparationMinutes,
                    MissingCount = recipe.RecipeIngredients
                        .Count(ri => !ingredientIds.Contains(ri.IngredientId)),
                    MissingIngredients = recipe.RecipeIngredients
                        .Where(ri => !ingredientIds.Contains(ri.IngredientId))
                        .Select(ri => ri.Ingredient)
                        .ToList()
                })
                .OrderBy(recipe => recipe.MissingCount)
                .ThenBy(recipe => recipe.Name)
                .ToListAsync()
        };

        return View(model);
    }

}

