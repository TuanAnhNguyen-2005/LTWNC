using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestfullAPI_NTHL.Models;

namespace RestfullAPI_NTHL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HocLieuController : ControllerBase
{
    private readonly NenTangDbContext _db;

    public HocLieuController(NenTangDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HocLieu>>> GetAll(
        [FromQuery] int? maChuDe,
        [FromQuery] int? maMonHoc,
        [FromQuery] int? maLopHoc)
    {
        var query = _db.HocLieu.AsNoTracking().AsQueryable();

        if (maChuDe.HasValue) query = query.Where(x => x.MaChuDe == maChuDe);
        if (maMonHoc.HasValue) query = query.Where(x => x.MaMonHoc == maMonHoc);
        if (maLopHoc.HasValue) query = query.Where(x => x.MaLopHoc == maLopHoc);

        var data = await query.ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HocLieu>> GetById(int id)
    {
        var item = await _db.HocLieu.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<HocLieu>> Create(HocLieu model)
    {
        _db.HocLieu.Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = model.MaHocLieu }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, HocLieu model)
    {
        if (id != model.MaHocLieu) return BadRequest("Id không khớp");

        var exists = await _db.HocLieu.AnyAsync(x => x.MaHocLieu == id);
        if (!exists) return NotFound();

        _db.Entry(model).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.HocLieu.FindAsync(id);
        if (item == null) return NotFound();

        _db.HocLieu.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

