using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nom_ws_datosempleado_be.Models;
using nom_ws_datosempleado_be.Data;

[Route("api/[controller]")]
[ApiController]
public class EstadoCivilsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public EstadoCivilsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/EstadoCivil
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstadoCivil>>> GetEstadoCivil()
    {
        return await _context.EstadosCiviles.ToListAsync();
    }

    // GET: api/EstadoCivil/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EstadoCivil>> GetEstadoCivil(long id)
    {
        var estadocivil = await _context.EstadosCiviles.FindAsync(id);

        if (estadocivil == null)
        {
            return NotFound();
        }

        return estadocivil;
    }

    // PUT: api/EstadoCivil/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEstadoCivil(long? id, EstadoCivil estadocivil)
    {
        if (id != estadocivil.Id)
        {
            return BadRequest();
        }

        _context.Entry(estadocivil).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EstadoCivilExists(id))
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

    // POST: api/EstadoCivil
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<EstadoCivil>> PostEstadoCivil(EstadoCivil estadocivil)
    {
        _context.EstadosCiviles.Add(estadocivil);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetEstadoCivil", new { id = estadocivil.Id }, estadocivil);
    }

    // DELETE: api/EstadoCivil/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEstadoCivil(long? id)
    {
        var estadocivil = await _context.EstadosCiviles.FindAsync(id);
        if (estadocivil == null)
        {
            return NotFound();
        }

        _context.EstadosCiviles.Remove(estadocivil);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EstadoCivilExists(long? id)
    {
        return _context.EstadosCiviles.Any(e => e.Id == id);
    }
}
