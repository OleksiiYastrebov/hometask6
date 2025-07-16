using System.ComponentModel.DataAnnotations.Schema;

namespace hometask6.Models;

public class Expense
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }

    [Column(TypeName = "decimal(10,2)")] 
    public decimal Price { get; set; }

    [Column(TypeName = "nvarchar(255)")] 
    public string? Comment { get; set; }

    public Category Category { get; set; } = null!;
}