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
    [HttpGet("debug/db")]
    public IActionResult DebugDb()
    {
        var conn = _db.Database.GetDbConnection();
        return Ok(new
        {
            dataSource = conn.DataSource,
            database = conn.Database,
            connectionString = _db.Database.GetConnectionString()
        });
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
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NguoiDung model)
    {
        if (model == null) return BadRequest("Body is null.");

        model.Email = model.Email?.Trim();

        // ✅ check trùng email
        var exists = await _db.NguoiDung.AnyAsync(x => x.Email == model.Email);
        if (exists)
            return Conflict(new { message = "Email đã tồn tại." }); // 409

        _db.NguoiDung.Add(model);
        await _db.SaveChangesAsync();

        return Ok(model);
    }

        /*
        return Ok(model);
        _db.NguoiDung.Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = model.MaNguoiDung }, model);*/

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


