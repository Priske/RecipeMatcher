using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipeMatcher.Web.Data;

namespace RecipeMatcher.Web.Tests;

public class RecipeValidationTests
{
    [Fact]
    public async Task Create_WithEmptyName_ReturnsErrorAndDoesNotSave()
    {
        using var factory =
            new CustomWebApplicationFactory();

        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        int countBefore;

        await using (var scope =
            factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            await db.Database.EnsureCreatedAsync();

            countBefore = await db.Recipes.CountAsync();
        }

        var formData = new Dictionary<string, string>
        {
            ["Name"] = "",
            ["PreparationMinutes"] = "20"
        };

        using var content =
            new FormUrlEncodedContent(formData);


        var response = await client.PostAsync(
            "/recipes/create",
            content);

        var html =
            await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<form", html);
        Assert.Contains(
            "The Name field is required.",
            html);


        await using (var scope =
            factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var countAfter =
                await db.Recipes.CountAsync();

            Assert.Equal(countBefore, countAfter);
        }
    }

    [Theory]
    [InlineData("0")]
    [InlineData("481")]
    public async Task Create_WithPreparationMinutesOutsideRange_DoesNotSave(
    string preparationMinutes)
    {
        using var factory =
            new CustomWebApplicationFactory();

        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        int countBefore;

        await using (var scope =
            factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            await db.Database.EnsureCreatedAsync();
            countBefore = await db.Recipes.CountAsync();
        }

        var formData = new Dictionary<string, string>
        {
            ["Name"] = "Valid Recipe Name",
            ["PreparationMinutes"] = preparationMinutes
        };

        using var content =
            new FormUrlEncodedContent(formData);

        var response = await client.PostAsync(
            "/recipes/create",
            content);

        var html =
            await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(
            "The field PreparationMinutes must be between 1 and 480.",
            html);

        await using (var scope =
            factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var countAfter = await db.Recipes.CountAsync();

            Assert.Equal(countBefore, countAfter);
        }
    }
}