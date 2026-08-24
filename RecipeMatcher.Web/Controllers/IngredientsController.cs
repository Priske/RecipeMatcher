using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeMatcher.Web.Data;
using RecipeMatcher.Web.Models;

namespace RecipeMatcher.Web.Controllers;

public class IngredientsController : Controller
{
    private readonly AppDbContext _dbContext;

    public IngredientsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var ingredients = await _dbContext.Ingredients
            .OrderBy(ingredients => ingredients.Name)
            .ToListAsync();

        return View(ingredients);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Ingredient ingredient)
    {
        if (!ModelState.IsValid)
        {
            return View(ingredient);
        }

        var nameExists = await _dbContext.Ingredients
            .AnyAsync(existing => existing.Name == ingredient.Name);

        if (nameExists)
        {
            ModelState.AddModelError(nameof(Ingredient.Name), "An ingredient with this name already exists.");
            return View(ingredient);
        }

        _dbContext.Ingredients.Add(ingredient);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var ingredient = await _dbContext.Ingredients.FindAsync(id);

        if (ingredient is null)
        {
            return NotFound();
        }

        return View(ingredient);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Ingredient ingredient)
    {
        var existingIngredient = await _dbContext.Ingredients.FindAsync(id);

        if (existingIngredient is null)
        {
            return NotFound();
        }

        ingredient.Id = id;

        if (!ModelState.IsValid)
        {
            return View(ingredient);
        }

        var nameExists = await _dbContext.Ingredients
            .AnyAsync(existing => existing.Id != id && existing.Name == ingredient.Name);

        if (nameExists)
        {
            ModelState.AddModelError(nameof(Ingredient.Name), "An ingredient with this name already exists.");
            return View(ingredient);
        }

        existingIngredient.Name = ingredient.Name;
        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var ingredient = await _dbContext.Ingredients.FindAsync(id);

        if (ingredient is null)
        {
            return NotFound();
        }
        return View(ingredient);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ingredient = await _dbContext.Ingredients.FindAsync(id);

        if (ingredient is null)
        {
            return NotFound();
        }

        _dbContext.Ingredients.Remove(ingredient);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}