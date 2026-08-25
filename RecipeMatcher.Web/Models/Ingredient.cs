using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RecipeMatcher.Web.Models;

[Index(nameof(Name), IsUnique = true)]
public class Ingredient
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]

    public string Name { get; set; } = "";

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
}



/*
MVC validation checks incoming user input and provides friendly error messages.
Database constraints enforce data integrity regardless of where the data came from.
Some annotations affect both MVC and EF, but not every MVC validation rule becomes a database constraint.
*/
