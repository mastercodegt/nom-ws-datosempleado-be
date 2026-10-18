using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nom_ws_datosempleado_be.Models;
using nom_ws_datosempleado_be.Data;

[Route("api/roles")]
[ApiController]
public class RolsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public RolsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Rol
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rol>>> GetRol()
    {
        return await _context.Roles.ToListAsync();
    }

    // GET: api/Rol/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Rol>> GetRol(long id)
    {
        var rol = await _context.Roles.FindAsync(id);

        if (rol == null)
        {
            return NotFound();
        }

        return rol;
    }

    // PUT: api/Rol/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRol(long? id, Rol rol)
    {
        if (id != rol.Id)
        {
            return BadRequest();
        }

        _context.Entry(rol).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RolExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Rol
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Rol>> PostRol(Rol rol)
    {
        _context.Roles.Add(rol);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetRol", new { id = rol.Id }, rol);
    }

    // DELETE: api/Rol/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRol(long? id)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null)
        {
            return NotFound();
        }

        _context.Roles.Remove(rol);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool RolExists(long? id)
    {
        return _context.Roles.Any(e => e.Id == id);
    }
}
