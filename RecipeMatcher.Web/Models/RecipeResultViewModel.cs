
namespace RecipeMatcher.Web.Models;

using System.ComponentModel.DataAnnotations;

public class RecipeResultViewModel
{
    public int RecipeId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [Range(1, 480)]
    public int PreparationMinutes { get; set; }

    public int MissingCount { get; set; }
    public IReadOnlyList<Ingredient> MissingIngredients { get; set; } = [];
}



