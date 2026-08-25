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
    public IActionResult Index(int[]? ingredientIds)
    {
        ingredientIds ??= [];

        if (ingredientIds.Length == 0)
        {
            return Content("No ingredient IDs selected.");
        }

        return Content(
            $"Selected ingredient IDs: {string.Join(", ", ingredientIds)}");
    }
}