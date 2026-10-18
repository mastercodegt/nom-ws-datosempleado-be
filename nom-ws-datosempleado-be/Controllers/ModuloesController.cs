using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nom_ws_datosempleado_be.Models;
using nom_ws_datosempleado_be.Data;

[Route("api/[controller]")]
[ApiController]
public class ModuloesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ModuloesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Modulo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Modulo>>> GetModulo()
    {
        return await _context.Modulos.ToListAsync();
    }

    // GET: api/Modulo/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Modulo>> GetModulo(long id)
    {
        var modulo = await _context.Modulos.FindAsync(id);

        if (modulo == null)
        {
            return NotFound();
        }

        return modulo;
    }

    // PUT: api/Modulo/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutModulo(long? id, Modulo modulo)
    {
        if (id != modulo.Id)
        {
            return BadRequest();
        }

        _context.Entry(modulo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ModuloExists(id))
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

    // POST: api/Modulo
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Modulo>> PostModulo(Modulo modulo)
    {
        _context.Modulos.Add(modulo);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetModulo", new { id = modulo.Id }, modulo);
    }

    // DELETE: api/Modulo/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteModulo(long? id)
    {
        var modulo = await _context.Modulos.FindAsync(id);
        if (modulo == null)
        {
            return NotFound();
        }

        _context.Modulos.Remove(modulo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ModuloExists(long? id)
    {
        return _context.Modulos.Any(e => e.Id == id);
    }
}
