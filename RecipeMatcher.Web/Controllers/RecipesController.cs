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
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var recipe = await _dbContext.Recipes.FindAsync(id);

        if (recipe is null)
        {
            return NotFound();
        }

        return View(recipe);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Recipe recipe)
    {
        if (!ModelState.IsValid)
        {
            return View(recipe);
        }

        _dbContext.Recipes.Add(recipe);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}