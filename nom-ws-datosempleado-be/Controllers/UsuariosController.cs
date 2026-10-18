using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nom_ws_datosempleado_be.Models;
using nom_ws_datosempleado_be.Data;
using System.Security.Cryptography;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public UsuariosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Usuario
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuario()
    {
        return await _context.Usuarios.ToListAsync();
    }

    // GET: api/Usuario/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> GetUsuario(long id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario == null)
        {
            return NotFound();
        }

        return usuario;
    }

    // PUT: api/Usuario/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUsuario(long? id, Usuario usuario)
    {
        if (id != usuario.Id)
        {
            return BadRequest();
        }
        // No procesar ni crear permisos desde el endpoint de usuario.
        // Actualizar solo los campos permitidos.
        var existing = await _context.Usuarios.FindAsync(id);
        if (existing == null)
            return NotFound();

        // Actualizar campos individuales
        existing.Nombre = usuario.Nombre;
        existing.Apellido = usuario.Apellido;
        existing.RolId = usuario.RolId;
        existing.Correo = usuario.Correo;
        existing.Estado = usuario.Estado;

        if (!string.IsNullOrWhiteSpace(usuario.Contrasena))
        {
            existing.Contrasena = HashSha256(usuario.Contrasena);
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UsuarioExists(id))
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

    // POST: api/Usuario
    [HttpPost]
    public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
    {
        if (!string.IsNullOrWhiteSpace(usuario.Contrasena))
        {
            usuario.Contrasena = HashSha256(usuario.Contrasena);
        }
        // Evitar crear permisos asociados desde este endpoint
        usuario.Permisos = null;

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
    }

    private static string HashSha256(string input)
    {
        if (input == null) return null;
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        var sb = new StringBuilder();
        foreach (var b in hash)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }

    // DELETE: api/Usuario/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsuario(long? id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            return NotFound();
        }

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UsuarioExists(long? id)
    {
        return _context.Usuarios.Any(e => e.Id == id);
    }
}
