using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipeMatcher.Web.Data;
using RecipeMatcher.Web.Models;

namespace RecipeMatcher.Web.Tests;

public class EditRecipeTests
{
    [Fact]
    public async Task GetEdit_ShowsExistingValues()
    {
        using var factory =
            new CustomWebApplicationFactory();

        var recipe = await AddRecipe(
            factory,
            "Pancakes",
            20);

        using var client = factory.CreateClient();

        var response =
            await client.GetAsync(
                $"/recipes/edit/{recipe.Id}");

        var html =
            await response.Content.ReadAsStringAsync();

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        Assert.Contains("Pancakes", html);
        Assert.Contains("20", html);
    }

    [Fact]
    public async Task PostEdit_UpdatesRecipe()
    {
        using var factory =
            new CustomWebApplicationFactory();

        var recipe = await AddRecipe(
            factory,
            "Old name",
            20);

        using var client = factory.CreateClient(
            new()
            {
                AllowAutoRedirect = false
            });

        var form = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["Name"] = "New name",
                ["PreparationMinutes"] = "30"
            });

        var response = await client.PostAsync(
            $"/recipes/edit/{recipe.Id}",
            form);

        Assert.Equal(
            HttpStatusCode.Redirect,
            response.StatusCode);

        using var scope =
            factory.Services.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        var updatedRecipe =
            await dbContext.Recipes
                .AsNoTracking()
                .SingleAsync(
                    recipe =>
                        recipe.Id == recipe.Id);

        Assert.Equal(
            "New name",
            updatedRecipe.Name);

        Assert.Equal(
            30,
            updatedRecipe.PreparationMinutes);
    }

    [Fact]
    public async Task PostEdit_InvalidData_DoesNotUpdateRecipe()
    {
        using var factory =
            new CustomWebApplicationFactory();

        var recipe = await AddRecipe(
            factory,
            "Original name",
            25);

        using var client = factory.CreateClient();

        var form = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["Name"] = "",
                ["PreparationMinutes"] = "25"
            });

        var response = await client.PostAsync(
            $"/recipes/edit/{recipe.Id}",
            form);

        var html =
            await response.Content.ReadAsStringAsync();

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        Assert.Contains(
            "The Name field is required.",
            html);

        using var scope =
            factory.Services.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        var unchangedRecipe =
            await dbContext.Recipes
                .AsNoTracking()
                .SingleAsync(
                    storedRecipe =>
                        storedRecipe.Id == recipe.Id);

        Assert.Equal(
            "Original name",
            unchangedRecipe.Name);

        Assert.Equal(
            25,
            unchangedRecipe.PreparationMinutes);
    }

    [Fact]
    public async Task GetEdit_UnknownId_ReturnsNotFound()
    {
        using var factory =
            new CustomWebApplicationFactory();

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext =
                scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

            await dbContext.Database.EnsureCreatedAsync();
        }

        using var client = factory.CreateClient();

        var response =
            await client.GetAsync(
                "/recipes/edit/999999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    private static async Task<Recipe> AddRecipe(
                CustomWebApplicationFactory factory,
        string name,
        int preparationMinutes)
    {
        using var scope =
            factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.EnsureCreatedAsync();

        var recipe = new Recipe
        {
            Name = name,
            PreparationMinutes = preparationMinutes
        };
        dbContext.Recipes.Add(recipe);
        await dbContext.SaveChangesAsync();

        return recipe;
    }
}