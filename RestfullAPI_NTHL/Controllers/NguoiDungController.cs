using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestfullAPI_NTHL.Models;

namespace RestfullAPI_NTHL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NguoiDungController : ControllerBase
{
    private readonly NenTangDbContext _db;

    public NguoiDungController(NenTangDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NguoiDung>>> GetAll()
    {
        var data = await _db.NguoiDung
            .AsNoTracking()
            .ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NguoiDung>> GetById(int id)
    {
        var item = await _db.NguoiDung.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<NguoiDung>> Create(NguoiDung model)
    {
        _db.NguoiDung.Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = model.MaNguoiDung }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, NguoiDung model)
    {
        if (id != model.MaNguoiDung) return BadRequest("Id không khớp");

        var exists = await _db.NguoiDung.AnyAsync(x => x.MaNguoiDung == id);
        if (!exists) return NotFound();

        _db.Entry(model).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.NguoiDung.FindAsync(id);
        if (item == null) return NotFound();

        _db.NguoiDung.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

