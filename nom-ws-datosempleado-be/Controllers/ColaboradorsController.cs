using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nom_ws_datosempleado_be.Models;
using nom_ws_datosempleado_be.Data;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class ColaboradorsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ColaboradorsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // DTO para respuestas que incluyen nombres de relaciones
    public class ColaboradorDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Direccion { get; set; }
        public int Edad { get; set; }
        public long ProfesionId { get; set; }
        public string ProfesionNombre { get; set; }
        public long EstadoCivilId { get; set; }
        public string EstadoCivilNombre { get; set; }
    }

    // GET: api/Colaborador
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ColaboradorDto>>> GetColaborador()
    {
        var list = await _context.Colaboradores
            .Include(c => c.Profesion)
            .Include(c => c.EstadoCivil)
            .Select(c => new ColaboradorDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Direccion = c.Direccion,
                Edad = c.Edad,
                ProfesionId = c.ProfesionId,
                ProfesionNombre = c.Profesion != null ? c.Profesion.Nombre : string.Empty,
                EstadoCivilId = c.EstadoCivilId,
                EstadoCivilNombre = c.EstadoCivil != null ? c.EstadoCivil.Nombre : string.Empty
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/Colaborador/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ColaboradorDto>> GetColaborador(long id)
    {
        var c = await _context.Colaboradores
            .Include(x => x.Profesion)
            .Include(x => x.EstadoCivil)
            .Where(x => x.Id == id)
            .Select(x => new ColaboradorDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Apellido = x.Apellido,
                Direccion = x.Direccion,
                Edad = x.Edad,
                ProfesionId = x.ProfesionId,
                ProfesionNombre = x.Profesion != null ? x.Profesion.Nombre : string.Empty,
                EstadoCivilId = x.EstadoCivilId,
                EstadoCivilNombre = x.EstadoCivil != null ? x.EstadoCivil.Nombre : string.Empty
            })
            .FirstOrDefaultAsync();

        if (c == null)
        {
            return NotFound();
        }

        return Ok(c);
    }

    // PUT: api/Colaborador/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutColaborador(long? id, Colaborador colaborador)
    {
        if (id != colaborador.Id)
        {
            return BadRequest();
        }

        _context.Entry(colaborador).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ColaboradorExists(id))
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

    // POST: api/Colaborador
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Colaborador>> PostColaborador(Colaborador colaborador)
    {
        _context.Colaboradores.Add(colaborador);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetColaborador", new { id = colaborador.Id }, colaborador);
    }

    // DELETE: api/Colaborador/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteColaborador(long? id)
    {
        var colaborador = await _context.Colaboradores.FindAsync(id);
        if (colaborador == null)
        {
            return NotFound();
        }

        _context.Colaboradores.Remove(colaborador);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ColaboradorExists(long? id)
    {
        return _context.Colaboradores.Any(e => e.Id == id);
    }
}
