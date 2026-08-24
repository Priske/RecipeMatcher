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
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Recipe recipe)
    {
        _dbContext.Recipes.Add(recipe);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}