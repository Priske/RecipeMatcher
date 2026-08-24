using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipeMatcher.Web.Data;
using RecipeMatcher.Web.Models;

namespace RecipeMatcher.Web.Tests;

public class RecipeDetailsTests
{
    [Fact]
    public async Task Details_WithExistingId_ReturnsRecipe()
    {
        using var factory =
            new CustomWebApplicationFactory();

        int recipeId;

        await using (var scope =
            factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            await db.Database.EnsureCreatedAsync();

            var recipe = new Recipe
            {
                Name = "Test Carbonara",
                PreparationMinutes = 37
            };

            db.Recipes.Add(recipe);
            await db.SaveChangesAsync();

            recipeId = recipe.Id;
        }

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/recipes/details/{recipeId}");

        var html =
            await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Test Carbonara", html);
        Assert.Contains("37", html);
    }

    [Fact]
    public async Task Details_WithUnknownId_ReturnsNotFound()
    {
        using var factory =
            new CustomWebApplicationFactory();

        await using (var scope =
            factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            await db.Database.EnsureCreatedAsync();
        }

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/recipes/details/999999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }
}