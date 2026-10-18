using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using nom_ws_datosempleado_be.Data;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

[Route("api/[controller]")]
[ApiController]
public class AuthenticateController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthenticateController> _logger;

    public AuthenticateController(ApplicationDbContext context, IConfiguration config, ILogger<AuthenticateController> logger)
    {
        _context = context;
        _config = config;
        _logger = logger;
    }

    public class LoginRequest
    {
        public string Correo { get; set; }
        public string Contrasena { get; set; }
    }

    public class LoginResponse
    {
        public long IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public long IdRol { get; set; }
        public string NombreRol { get; set; }
        public string Token { get; set; }
    }

    [HttpPost]
    public async Task<ActionResult<LoginResponse>> Post([FromBody] LoginRequest req)
    {
        if (req == null || string.IsNullOrWhiteSpace(req.Correo) || string.IsNullOrWhiteSpace(req.Contrasena))
            return BadRequest();

        try
        {
            // Llamar a la función de la base de datos
            await using var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id_usuario, nombre_completo, id_rol, nombre_rol, autenticado FROM fn_validar_usuario(@p_correo, @p_contrasena)";

            var p1 = cmd.CreateParameter();
            p1.ParameterName = "@p_correo";
            p1.Value = req.Correo;
            cmd.Parameters.Add(p1);

            var p2 = cmd.CreateParameter();
            p2.ParameterName = "@p_contrasena";
            p2.Value = req.Contrasena;
            cmd.Parameters.Add(p2);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return Unauthorized();
            }

            var autenticado = reader.GetBoolean(reader.GetOrdinal("autenticado"));
            if (!autenticado)
                return Unauthorized();

            var idUsuario = reader.GetInt64(reader.GetOrdinal("id_usuario"));
            var nombreCompleto = reader.IsDBNull(reader.GetOrdinal("nombre_completo")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombre_completo"));
            var idRol = reader.GetInt64(reader.GetOrdinal("id_rol"));
            var nombreRol = reader.IsDBNull(reader.GetOrdinal("nombre_rol")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombre_rol"));

            // Generar JWT
            var token = GenerateJwt(idUsuario);

            var resp = new LoginResponse
            {
                IdUsuario = idUsuario,
                NombreCompleto = nombreCompleto,
                IdRol = idRol,
                NombreRol = nombreRol,
                Token = token
            };

            return Ok(resp);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error en AuthenticateController.Post");
            // Devolver información mínima; para depuración puedes devolver ex.Message temporalmente
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }

    private string GenerateJwt(long idUsuario)
    {
        // Obtener clave desde configuración: Jwt:Key
        var key = _config["Jwt:Key"] ?? "default_change_me_secret";
        var header = new { alg = "HS256", typ = "JWT" };
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var exp = DateTimeOffset.UtcNow.AddHours(8).ToUnixTimeSeconds();

        var payload = new Dictionary<string, object>
        {
            { "id_usuario", idUsuario },
            { "iat", now },
            { "exp", exp }
        };

        string headerJson = JsonSerializer.Serialize(header);
        string payloadJson = JsonSerializer.Serialize(payload);

        string headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        string payloadBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

        var unsignedToken = headerBase64 + "." + payloadBase64;

        var signature = Sign(unsignedToken, key);
        return unsignedToken + "." + signature;
    }

    private static string Sign(string unsignedToken, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        using var hmac = new HMACSHA256(keyBytes);
        var sig = hmac.ComputeHash(Encoding.UTF8.GetBytes(unsignedToken));
        return Base64UrlEncode(sig);
    }

    private static string Base64UrlEncode(byte[] input)
    {
        var s = Convert.ToBase64String(input);
        s = s.Split('=')[0]; // eliminar padding
        s = s.Replace('+', '-');
        s = s.Replace('/', '_');
        return s;
    }
}
