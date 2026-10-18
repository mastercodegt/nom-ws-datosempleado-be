using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nom_ws_datosempleado_be.Models;
using nom_ws_datosempleado_be.Data;

[Route("api/[controller]")]
[ApiController]
public class ProfesionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ProfesionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Profesion
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Profesion>>> GetProfesion()
    {
        return await _context.Profesiones.ToListAsync();
    }

    // GET: api/Profesion/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Profesion>> GetProfesion(long id)
    {
        var profesion = await _context.Profesiones.FindAsync(id);

        if (profesion == null)
        {
            return NotFound();
        }

        return profesion;
    }

    // PUT: api/Profesion/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProfesion(long? id, Profesion profesion)
    {
        if (id != profesion.Id)
        {
            return BadRequest();
        }

        _context.Entry(profesion).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProfesionExists(id))
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

    // POST: api/Profesion
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Profesion>> PostProfesion(Profesion profesion)
    {
        _context.Profesiones.Add(profesion);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProfesion", new { id = profesion.Id }, profesion);
    }

    // DELETE: api/Profesion/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProfesion(long? id)
    {
        var profesion = await _context.Profesiones.FindAsync(id);
        if (profesion == null)
        {
            return NotFound();
        }

        _context.Profesiones.Remove(profesion);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProfesionExists(long? id)
    {
        return _context.Profesiones.Any(e => e.Id == id);
    }
}
