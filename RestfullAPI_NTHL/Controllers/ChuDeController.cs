using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestfullAPI_NTHL.Models;

namespace RestfullAPI_NTHL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChuDeController : ControllerBase
{
    private readonly NenTangDbContext _db;

    public ChuDeController(NenTangDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChuDe>>> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        if (take < 1) take = 20;
        if (take > 100) take = 100; // Bảo vệ query to quá
        var total = await _db.ChuDe.CountAsync();
        var data = await _db.ChuDe.AsNoTracking().Skip(skip).Take(take).ToListAsync();
        Response.Headers.Add("X-Total-Count", total.ToString());
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ChuDe>> GetById(int id)
    {
        var item = await _db.ChuDe.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<ChuDe>> Create(ChuDe model)
    {
        _db.ChuDe.Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = model.MaChuDe }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ChuDe model)
    {
        if (id != model.MaChuDe) return BadRequest("Id không khớp");

        var exists = await _db.ChuDe.AnyAsync(x => x.MaChuDe == id);
        if (!exists) return NotFound();

        _db.Entry(model).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.ChuDe.FindAsync(id);
        if (item == null) return NotFound();

        _db.ChuDe.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}


