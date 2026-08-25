using Microsoft.EntityFrameworkCore;
namespace RecipeMatcher.Web.Models;

[PrimaryKey(nameof(RecipeId), nameof(IngredientId))]
public class RecipeIngredient
{
    public int RecipeId { get; set; }

    public Recipe Recipe { get; set; } = null!;

    public int IngredientId { get; set; }

    public Ingredient Ingredient { get; set; } = null!;
}