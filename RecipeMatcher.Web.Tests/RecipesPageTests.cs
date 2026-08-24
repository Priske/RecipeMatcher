using System.Net;
using Microsoft.Extensions.DependencyInjection;
using RecipeMatcher.Web.Data;
using RecipeMatcher.Web.Models;

namespace RecipeMatcher.Web.Tests;

public class RecipesPageTests
{
    [Fact]
    public async Task GetRecipes_ReturnsOkAndDisplaysRecipe()
    {

        using var factory =
            new CustomWebApplicationFactory();

        await using (var scope =
            factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            await db.Database.EnsureCreatedAsync();

            db.Recipes.Add(new Recipe
            {
                Name = "Integration Test Lasagna",
                PreparationMinutes = 45
            });

            await db.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/recipes");

        var html =
            await response.Content.ReadAsStringAsync();


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Recipes", html);
        Assert.Contains("Integration Test Lasagna", html);
        Assert.Contains("45 minutes", html);
    }
}