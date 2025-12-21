using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestfullAPI_NTHL.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace RestfullAPI_NTHL.Controllers;
[ApiController]
[Route("categories")] // route cũ vẫn giữ
public class CategoriesController : ControllerBase
{
    private readonly NenTangDbContext _context;

    public CategoriesController(NenTangDbContext context)
    {
        _context = context;
    }

    // GET /categories  +  GET /api/categories
    [HttpGet]
    [HttpGet("/api/categories")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.Categories
            .Select(c => new
            {
                categoryId = c.CategoryId,
                categoryName = c.CategoryName,
                slug = c.Slug,
                description = c.Description,
                courseCount = 0
            })
            .ToListAsync();

        return Ok(data);
    }

    // GET /categories/{id}  +  GET /api/categories/{id}
    [HttpGet("{id:int}")]
    [HttpGet("/api/categories/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _context.Categories.FindAsync(id);
        if (c == null) return NotFound();

        return Ok(new
        {
            categoryId = c.CategoryId,
            categoryName = c.CategoryName,
            slug = c.Slug,
            description = c.Description,
            isActive = c.IsActive
        });
    }

    // POST /categories  +  POST /api/categories
    [HttpPost]
    [HttpPost("/api/categories")]
    public async Task<IActionResult> Create([FromBody] Category model)
    {
        _context.Categories.Add(model);
        await _context.SaveChangesAsync();
        return Ok(model);
    }

    // PUT /categories/{id}  +  PUT /api/categories/{id}
    [HttpPut("{id:int}")]
    [HttpPut("/api/categories/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Category model)
    {
        var c = await _context.Categories.FindAsync(id);
        if (c == null) return NotFound();

        c.CategoryName = model.CategoryName;
        c.Slug = model.Slug;
        c.Description = model.Description;
        c.IsActive = model.IsActive;

        await _context.SaveChangesAsync();
        return Ok(c);
    }

    // DELETE /categories/{id}  +  DELETE /api/categories/{id}
    [HttpDelete("{id:int}")]
    [HttpDelete("/api/categories/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _context.Categories.FindAsync(id);
        if (c == null) return NotFound();

        _context.Categories.Remove(c);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
