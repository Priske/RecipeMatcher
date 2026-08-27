namespace RecipeMatcher.Web.Models;

public class MatcherViewModel
{
    public IReadOnlyList<IngredientOptionViewModel> Ingredients { get; set; } = [];
    public IReadOnlyList<RecipeResultViewModel> Recipes { get; set; } = [];
}