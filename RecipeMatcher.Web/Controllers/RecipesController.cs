using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeMatcher.Web.Data;
using RecipeMatcher.Web.Models;

namespace RecipeMatcher.Web.Controllers;

public class RecipesController : Controller
{
    private readonly AppDbContext _dbContext;

    public RecipesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var recipes = await _dbContext.Recipes
            .OrderBy(recipe => recipe.Name)
            .ToListAsync();

        return View(recipes);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Ingredients = await _dbContext.Ingredients.OrderBy(i => i.Name).ToListAsync();
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var recipe = await _dbContext.Recipes
            .Include(recipe => recipe.RecipeIngredients)
            .ThenInclude(recipeIngredient => recipeIngredient.Ingredient)
            .SingleOrDefaultAsync(recipe => recipe.Id == id);

        if (recipe is null)
        {
            return NotFound();
        }

        return View(recipe);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Recipe recipe, int[] ingredientIds)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Ingredients = await _dbContext.Ingredients.OrderBy(i => i.Name).ToListAsync();
            return View(recipe);
        }

        foreach (var ingredientId in ingredientIds)
        {
            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                IngredientId = ingredientId
            });
        }

        _dbContext.Recipes.Add(recipe);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var recipe = await _dbContext.Recipes
        .Include(recipe => recipe.RecipeIngredients)
        .SingleOrDefaultAsync(recipe => recipe.Id == id);

        if (recipe is null)
        {
            return NotFound();
        }

        var ingredients = await _dbContext.Ingredients
            .OrderBy(ingredient => ingredient.Name)
            .ToListAsync();

        var model = new EditRecipeViewModel
        {
            Id = recipe.Id,
            Name = recipe.Name,
            PreparationMinutes = recipe.PreparationMinutes,
            Ingredients = ingredients.Select(ingredient => new IngredientOptionViewModel
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Selected = recipe.RecipeIngredients.Any(
                    recipeIngredient => recipeIngredient.IngredientId == ingredient.Id)
            }).ToList()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        int id,
        EditRecipeViewModel model,
        int[] ingredientIds)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var existingRecipe = await _dbContext.Recipes
            .Include(recipe => recipe.RecipeIngredients)
            .SingleOrDefaultAsync(recipe => recipe.Id == id);

        if (existingRecipe is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            var selectedIds = ingredientIds.ToHashSet();

            var ingredients = await _dbContext.Ingredients
                .OrderBy(ingredient => ingredient.Name)
                .ToListAsync();

            model.Ingredients = ingredients
                .Select(ingredient => new IngredientOptionViewModel
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    Selected = selectedIds.Contains(ingredient.Id)
                })
                .ToList();

            return View(model);
        }

        existingRecipe.Name = model.Name;
        existingRecipe.PreparationMinutes = model.PreparationMinutes;

        var selectedIngredientIds = ingredientIds.ToHashSet();

        var currentIngredientIds = existingRecipe.RecipeIngredients
            .Select(recipeIngredient => recipeIngredient.IngredientId)
            .ToHashSet();

        var removedIngredients = existingRecipe.RecipeIngredients
            .Where(recipeIngredient =>
                !selectedIngredientIds.Contains(recipeIngredient.IngredientId))
            .ToList();

        _dbContext.RecipeIngredients.RemoveRange(removedIngredients);

        foreach (var ingredientId in selectedIngredientIds.Except(currentIngredientIds))
        {
            existingRecipe.RecipeIngredients.Add(new RecipeIngredient
            {
                RecipeId = existingRecipe.Id,
                IngredientId = ingredientId
            });
        }

        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var recipe = await _dbContext.Recipes.FindAsync(id);

        if (recipe is null)
        {
            return NotFound();
        }

        return View(recipe);
    }

    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var recipe = await _dbContext.Recipes.FindAsync(id);

        if (recipe is null)
        {
            return NotFound();
        }

        _dbContext.Recipes.Remove(recipe);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}