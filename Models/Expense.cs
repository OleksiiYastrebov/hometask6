using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hometask6.Models;

public class Expense
{
    public enum ExpenseCategory
    {
        Food,
        Transport,
        MobileCommunication,
        Internet,
        Entertainment
    }

    [Key] public Guid Id { get; set; }

    [Range(0, 4, ErrorMessage = "Value must be in range [0, 4]")]
    public ExpenseCategory Category { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    [Range(0.01, 999999.99, ErrorMessage = "Value must be in range [0.01-999999.99]")]
    public decimal Price { get; set; }

    [Column(TypeName = "nvarchar(255)")] public string? Comment { get; set; }
}