using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestFullAPI.DTOs;
using RestFullAPI.Models;

namespace RestFullAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    CategoryId = c.MaChuDe,
                    CategoryName = c.TenChuDe,
                    Description = c.MoTa,
                    Slug = GenerateSlug(c.TenChuDe),
                    IsActive = true
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(new { error = "Không tìm thấy danh mục" });
            }

            var categoryDto = new CategoryDto
            {
                CategoryId = category.MaChuDe,
                CategoryName = category.TenChuDe,
                Description = category.MoTa,
                Slug = GenerateSlug(category.TenChuDe),
                IsActive = true
            };

            return Ok(categoryDto);
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var category = new Category
            {
                TenChuDe = createCategoryDto.CategoryName,
                MoTa = createCategoryDto.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var categoryDto = new CategoryDto
            {
                CategoryId = category.MaChuDe,
                CategoryName = category.TenChuDe,
                Description = category.MoTa,
                Slug = createCategoryDto.Slug ?? GenerateSlug(category.TenChuDe),
                IsActive = createCategoryDto.IsActive
            };

            return CreatedAtAction(nameof(GetCategory), new { id = category.MaChuDe }, categoryDto);
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { error = "Không tìm thấy danh mục" });
            }

            if (!string.IsNullOrEmpty(updateCategoryDto.CategoryName))
                category.TenChuDe = updateCategoryDto.CategoryName;

            if (updateCategoryDto.Description != null)
                category.MoTa = updateCategoryDto.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CategoryExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { error = "Không tìm thấy danh mục" });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> CategoryExists(int id)
        {
            return await _context.Categories.AnyAsync(e => e.MaChuDe == id);
        }

        private string GenerateSlug(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Chuyển đổi tiếng Việt có dấu thành không dấu
            var slug = text.ToLower()
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray();

            var result = new string(slug);
            result = result.Replace("đ", "d").Replace("Đ", "d");
            result = System.Text.RegularExpressions.Regex.Replace(result, @"[^a-z0-9]+", "-");
            result = result.Trim('-');

            return result;
        }
    }
}

