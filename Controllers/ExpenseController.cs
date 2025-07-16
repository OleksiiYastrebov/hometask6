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
        var expenses = await _context.Expenses.Select(e => e).ToListAsync();
        return Ok(expenses);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> FindOneAsync(Guid id)
    {
        return Ok(await _context.Expenses.FindAsync(id));
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
                Comment = exp.Comment
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
}