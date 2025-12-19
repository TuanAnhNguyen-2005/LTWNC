using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestfullAPI_NTHL.Models;

namespace RestfullAPI_NTHL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VaiTroController : ControllerBase
{
    private readonly NenTangDbContext _db;

    public VaiTroController(NenTangDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VaiTro>>> GetAll()
    {
        var data = await _db.VaiTro.AsNoTracking().ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VaiTro>> GetById(int id)
    {
        var item = await _db.VaiTro.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<VaiTro>> Create(VaiTro model)
    {
        _db.VaiTro.Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = model.MaVaiTro }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, VaiTro model)
    {
        if (id != model.MaVaiTro) return BadRequest("Id không khớp");

        var exists = await _db.VaiTro.AnyAsync(x => x.MaVaiTro == id);
        if (!exists) return NotFound();

        _db.Entry(model).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.VaiTro.FindAsync(id);
        if (item == null) return NotFound();

        _db.VaiTro.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}


