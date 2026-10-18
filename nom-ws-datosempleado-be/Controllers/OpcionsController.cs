using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nom_ws_datosempleado_be.Models;
using nom_ws_datosempleado_be.Data;

[Route("api/[controller]")]
[ApiController]
public class OpcionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public OpcionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Opcion
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Opcion>>> GetOpcion()
    {
        return await _context.Opciones.ToListAsync();
    }

    // GET: api/Opcion/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Opcion>> GetOpcion(long id)
    {
        var opcion = await _context.Opciones.FindAsync(id);

        if (opcion == null)
        {
            return NotFound();
        }

        return opcion;
    }

    // PUT: api/Opcion/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutOpcion(long? id, Opcion opcion)
    {
        if (id != opcion.Id)
        {
            return BadRequest();
        }

        _context.Entry(opcion).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OpcionExists(id))
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

    // POST: api/Opcion
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Opcion>> PostOpcion(Opcion opcion)
    {
        _context.Opciones.Add(opcion);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetOpcion", new { id = opcion.Id }, opcion);
    }

    // DELETE: api/Opcion/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOpcion(long? id)
    {
        var opcion = await _context.Opciones.FindAsync(id);
        if (opcion == null)
        {
            return NotFound();
        }

        _context.Opciones.Remove(opcion);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool OpcionExists(long? id)
    {
        return _context.Opciones.Any(e => e.Id == id);
    }
}
