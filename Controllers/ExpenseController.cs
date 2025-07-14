using hometask6.Data;
using hometask6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hometask6.Controllers;

[Route("api/expenses")]
[ApiController]
public class ExpenseController : ControllerBase
{
    private readonly ApplicationContext _context;

    public ExpenseController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index()
    {
        var expenses = await _context.Expenses.Select(e => e).ToListAsync();
        return Ok(expenses);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Find(Guid id)
    {
        return Ok(await _context.Expenses.FindAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Expense exp)
    {
        if (!ModelState.IsValid) return BadRequest();

        _context.Expenses.Add(exp);
        await _context.SaveChangesAsync();

        return Created();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Expense exp)
    {
        if (!ModelState.IsValid) return BadRequest();

        var dbExpense = await _context.Expenses.FindAsync(exp.Id);

        dbExpense!.Category = exp.Category;
        dbExpense.Price = exp.Price;
        dbExpense.Comment = exp.Comment;

        await _context.SaveChangesAsync();

        return Ok();
    }
}