using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestfullAPI_NTHL.Models;

namespace RestfullAPI_NTHL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonHocController : ControllerBase
{
    private readonly NenTangDbContext _db;

    public MonHocController(NenTangDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MonHoc>>> GetAll()
    {
        var data = await _db.MonHoc.AsNoTracking().ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MonHoc>> GetById(int id)
    {
        var item = await _db.MonHoc.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<MonHoc>> Create(MonHoc model)
    {
        _db.MonHoc.Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = model.MaMonHoc }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, MonHoc model)
    {
        if (id != model.MaMonHoc) return BadRequest("Id không khớp");

        var exists = await _db.MonHoc.AnyAsync(x => x.MaMonHoc == id);
        if (!exists) return NotFound();

        _db.Entry(model).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.MonHoc.FindAsync(id);
        if (item == null) return NotFound();

        _db.MonHoc.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}


