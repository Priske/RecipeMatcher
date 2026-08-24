using RecipeMatcher.Web.Models;

namespace RecipeMatcher.Web.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.Recipes.Any())
        {
            return;
        }

        var recipes = new Recipe[]
        {
            new()
            {
                Name = "Basic Cheese Pizza",
                PreparationMinutes = 10
            },
            new()
            {
                Name = "Spaghetti",
                PreparationMinutes = 20
            },
            new()
            {
                Name = "Bacon & eggs",
                PreparationMinutes = 5
            },
            new()
            {
                Name = "Stront me bonen",
                PreparationMinutes = 0
            }
        };

        db.Recipes.AddRange(recipes);
        db.SaveChanges();
    }
}