using System.Text;
using FluentValidation;
using hometask6.Data;
using hometask6.Dtos;
using hometask6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hometask6.Controllers;

[Route("api/expenses")]
[ApiController]
public class ExpenseController : ControllerBase
{
    private readonly ApplicationContext _context;
    private readonly IValidator<ExpenseRequestDto> _validator;

    public ExpenseController(ApplicationContext context, IValidator<ExpenseRequestDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> FindAllAsync()
    {
        return Ok(
            await _context.Expenses
            .Select(e => new ExpenseResponseDto(e.Id, e.CategoryId, e.Price, e.Comment, e.DateTime))
            .ToListAsync()
            );
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> FindOneAsync(Guid id)
    {
        var exp = await _context.Expenses.FindAsync(id);
        
        if (exp is null)
        {
            return Empty;
        } 
        
        return Ok(new ExpenseResponseDto(exp.Id, exp.CategoryId, exp.Price, exp.Comment, exp.DateTime));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(ExpenseRequestDto exp)
    {
        var validationRes = await _validator.ValidateAsync(exp);
        if (!validationRes.IsValid)
        {
            var stringBuilder = new StringBuilder();
            foreach (var error in validationRes.Errors)
            {
                stringBuilder.Append(error.ErrorMessage);
                stringBuilder.Append('\n');
            }

            return BadRequest(stringBuilder.ToString());
        }
        
        if (await _context.Categories.FindAsync(exp.CategoryId) is null)
        {
            return BadRequest("There is no category with such ID");
        }
        
        _context.Expenses.Add
        (
            new Expense
            {
                Id = Guid.CreateVersion7(),
                CategoryId = exp.CategoryId,
                Price = exp.Price,
                Comment = exp.Comment,
                DateTime = exp.DateTime ?? DateTime.Now
            }
        );
        
        await _context.SaveChangesAsync();

        return Created();
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(ExpenseRequestDto exp, Guid id)
    {
        var validationRes = await _validator.ValidateAsync(exp);
        if (!validationRes.IsValid)
        {
            var stringBuilder = new StringBuilder();
            foreach (var error in validationRes.Errors)
            {
                stringBuilder.Append(error.ErrorMessage);
                stringBuilder.Append('\n');
            }

            return BadRequest(stringBuilder.ToString());
        }
    
        if (await _context.Categories.FindAsync(exp.CategoryId) is null)
        {
            return BadRequest("There is no category with such ID");
        }
        
        var dbExp = await _context.Expenses.FindAsync(id);

        dbExp!.CategoryId = exp.CategoryId;
        dbExp.Price = exp.Price;
        dbExp.Comment = exp.Comment;

        await _context.SaveChangesAsync();

        return Ok();
    }
    
    [HttpGet]
    [Route("month")]
    public async Task<IActionResult> GetExpensesByMonth([FromQuery] DateTime fromDate)
    {
        if (fromDate.Equals(new DateTime()))
        {
            fromDate = DateTime.Today;
        }
        
        var targetMonth = fromDate.Month;
        var targetYear = fromDate.Year;

        var expenses = await _context.Expenses
            .AsNoTracking()
            .Where(e => e.DateTime.Month == targetMonth && e.DateTime.Year == targetYear)
            .Include(e => e.Category)
            .OrderBy(e => e.DateTime)
            .Select(e => new
            {
                e.Id,
                e.Price,
                e.Comment,
                e.DateTime,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name
            })
            .ToListAsync();
        
        return Ok(expenses);
    }
    
    [HttpGet]
    [Route("monthly-summary")]
    public async Task<IActionResult> GetMonthlySummary([FromQuery] DateTime fromDate)
    {
        if (fromDate.Equals(new DateTime()))
        {
            fromDate = DateTime.Today;
        }
        
        var targetMonth = fromDate.Month;
        var targetYear = fromDate.Year;

        var result = await _context.Expenses
            .AsNoTracking()
            .Where(e => e.DateTime.Month == targetMonth && e.DateTime.Year == targetYear)
            .GroupBy(e => new { e.CategoryId, e.Category.Name }) // Use navigation property directly
            .Select(g => new
            {
                categoryId = g.Key.CategoryId,
                categoryName = g.Key.Name,
                total = g.Sum(e => e.Price)
            })
            .ToListAsync();

        return Ok(result);
    }

}