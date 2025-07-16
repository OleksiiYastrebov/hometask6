using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hometask6.Models;

[Index(nameof(Name), IsUnique = true)]
public class Category
{
    public Guid Id { get; set; }

    [Column(TypeName = "nvarchar(100)")] 
    public string Name { get; set; } = null!;

    public ICollection<Expense>? Expenses { get; set; } = new List<Expense>();
}