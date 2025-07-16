using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hometask6.Dtos;

public class ExpenseRequestDto
{
    public Guid CategoryId { get; set; }
    public decimal Price { get; set; }
    public string? Comment { get; set; }
}