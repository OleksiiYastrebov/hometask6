using System.Text;
using FluentValidation;
using hometask6.Data;
using hometask6.Dtos;
using hometask6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hometask6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ApplicationContext _context;
    private readonly IValidator<CategoryRequestDto> _validator;

    public CategoryController(ApplicationContext context, IValidator<CategoryRequestDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(
            await _context.Categories
                .Select(c => new CategoryResponseDto(c.Id, c.Name))
                .ToListAsync()
            );
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CategoryRequestDto category)
    {
        var validationRes = await _validator.ValidateAsync(category);
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

        try
        {
            await _context.AddAsync(
                new Category
                {
                    Id = Guid.CreateVersion7(),
                    Name = category.Name
                }
            );
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return BadRequest("Duplicate name for categories not allowed");
        }

        return Created();
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(CategoryRequestDto category,Guid id)
    {
        var validationRes = await _validator.ValidateAsync(category);
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
        
        var res = await _context.Categories.FindAsync(id);
        
        if (res is null)
        {
            return BadRequest("There is no category with such id");
        }

        try
        {
            var dbCategory = await _context.Categories.FindAsync(id);
            dbCategory!.Name = category.Name;

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return BadRequest("Duplicate name for categories not allowed");
        }

        return Ok();
    }

    
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var res = await _context.Categories.FindAsync(id);
        
        if (res is null)
        {
            return BadRequest("There is no category with such id");
        }

        try
        {
            _context.Categories.Remove(res);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Can`t delete this category because some expenses use it");
        }
        
        return Ok();
    }
}