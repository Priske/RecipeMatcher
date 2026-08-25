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
        var recipe = await _dbContext.Recipes.FindAsync(id);

        if (recipe is null)
        {
            return NotFound();
        }

        return View(recipe);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Recipe recipe)
    {
        if (id != recipe.Id)
        {
            return BadRequest();
        }

        var existingRecipe = await _dbContext.Recipes.FindAsync(id);

        if (existingRecipe is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(recipe);
        }

        existingRecipe.Name = recipe.Name;
        existingRecipe.PreparationMinutes = recipe.PreparationMinutes;

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