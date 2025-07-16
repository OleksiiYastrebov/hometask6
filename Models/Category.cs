using System.ComponentModel.DataAnnotations.Schema;

namespace hometask6.Models;

public class Category
{
    public Guid Id { get; set; }

    [Column(TypeName = "nvarchar(100)")] 
    public string Name { get; set; } = null!;

    public ICollection<Expense>? Expenses { get; set; } = new List<Expense>();
}