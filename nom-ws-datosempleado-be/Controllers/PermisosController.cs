using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nom_ws_datosempleado_be.Models;
using nom_ws_datosempleado_be.Data;

[Route("api/[controller]")]
[ApiController]
public class PermisosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public PermisosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Permiso
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Permiso>>> GetPermiso()
    {
        return await _context.Permisos.ToListAsync();
    }

    // GET: api/Permiso/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Permiso>> GetPermiso(long id)
    {
        var permiso = await _context.Permisos.FindAsync(id);

        if (permiso == null)
        {
            return NotFound();
        }

        return permiso;
    }

    // PUT: api/Permiso/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPermiso(long? id, Permiso permiso)
    {
        if (id != permiso.Id)
        {
            return BadRequest();
        }

        _context.Entry(permiso).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PermisoExists(id))
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

    // POST: api/Permiso
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Permiso>> PostPermiso(Permiso permiso)
    {
        _context.Permisos.Add(permiso);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetPermiso", new { id = permiso.Id }, permiso);
    }

    // DELETE: api/Permiso/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePermiso(long? id)
    {
        var permiso = await _context.Permisos.FindAsync(id);
        if (permiso == null)
        {
            return NotFound();
        }

        _context.Permisos.Remove(permiso);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PermisoExists(long? id)
    {
        return _context.Permisos.Any(e => e.Id == id);
    }
}
