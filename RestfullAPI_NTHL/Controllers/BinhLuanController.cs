using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestfullAPI_NTHL.Models;

namespace RestfullAPI_NTHL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BinhLuanController : ControllerBase
{
    private readonly NenTangDbContext _db;

    public BinhLuanController(NenTangDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BinhLuan>>> GetAll([FromQuery] int? maHocLieu)
    {
        var query = _db.BinhLuan.AsNoTracking().AsQueryable();
        if (maHocLieu.HasValue)
        {
            query = query.Where(x => x.MaHocLieu == maHocLieu);
        }

        var data = await query.ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BinhLuan>> GetById(int id)
    {
        var item = await _db.BinhLuan.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<BinhLuan>> Create(BinhLuan model)
    {
        _db.BinhLuan.Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = model.MaBinhLuan }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, BinhLuan model)
    {
        if (id != model.MaBinhLuan) return BadRequest("Id không khớp");

        var exists = await _db.BinhLuan.AnyAsync(x => x.MaBinhLuan == id);
        if (!exists) return NotFound();

        _db.Entry(model).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.BinhLuan.FindAsync(id);
        if (item == null) return NotFound();

        _db.BinhLuan.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}


