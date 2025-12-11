using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestfullAPI_NTHL.Models;

namespace RestfullAPI_NTHL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LopHocController : ControllerBase
{
    private readonly NenTangDbContext _db;

    public LopHocController(NenTangDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LopHoc>>> GetAll()
    {
        var data = await _db.LopHoc.AsNoTracking().ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LopHoc>> GetById(int id)
    {
        var item = await _db.LopHoc.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<LopHoc>> Create(LopHoc model)
    {
        _db.LopHoc.Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = model.MaLopHoc }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, LopHoc model)
    {
        if (id != model.MaLopHoc) return BadRequest("Id không khớp");

        var exists = await _db.LopHoc.AnyAsync(x => x.MaLopHoc == id);
        if (!exists) return NotFound();

        _db.Entry(model).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.LopHoc.FindAsync(id);
        if (item == null) return NotFound();

        _db.LopHoc.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

