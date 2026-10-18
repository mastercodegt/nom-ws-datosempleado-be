using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace nom_ws_datosempleado_be.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public JwtMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = GetTokenFromRequest(context);
            if (string.IsNullOrEmpty(token))
            {
                await _next(context);
                return;
            }

            if (!ValidateToken(token, out long idUsuario))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token");
                return;
            }

            // Attach id_usuario to HttpContext.Items and set ClaimsPrincipal
            context.Items["IdUsuario"] = idUsuario;
            var claims = new[] { new Claim("id_usuario", idUsuario.ToString()) };
            context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

            await _next(context);
        }

        private string GetTokenFromRequest(HttpContext context)
        {
            var auth = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(auth) && auth.StartsWith("Bearer "))
            {
                return auth.Substring("Bearer ".Length).Trim();
            }

            // Fallback to query string
            if (context.Request.Query.TryGetValue("token", out var t))
            {
                return t.FirstOrDefault();
            }

            return null;
        }

        private bool ValidateToken(string token, out long idUsuario)
        {
            idUsuario = 0;
            try
            {
                var parts = token.Split('.');
                if (parts.Length != 3) return false;

                var unsigned = parts[0] + "." + parts[1];
                var signature = parts[2];

                var key = _config["Jwt:Key"] ?? "default_change_me_secret";
                var expectedSig = Sign(unsigned, key);
                if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(signature), Encoding.UTF8.GetBytes(expectedSig)))
                {
                    return false;
                }

                var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
                using var doc = JsonDocument.Parse(payloadJson);
                var root = doc.RootElement;

                if (root.TryGetProperty("exp", out var expProp))
                {
                    var exp = expProp.GetInt64();
                    var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    if (now >= exp) return false;
                }

                if (root.TryGetProperty("id_usuario", out var idProp))
                {
                    if (idProp.ValueKind == JsonValueKind.Number && idProp.TryGetInt64(out var v))
                    {
                        idUsuario = v;
                        return true;
                    }
                    if (idProp.ValueKind == JsonValueKind.String && long.TryParse(idProp.GetString(), out var v2))
                    {
                        idUsuario = v2;
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private static byte[] Base64UrlDecode(string input)
        {
            var s = input;
            s = s.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 2: s += "=="; break;
                case 3: s += "="; break;
            }
            return Convert.FromBase64String(s);
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
            s = s.Split('=')[0];
            s = s.Replace('+', '-');
            s = s.Replace('/', '_');
            return s;
        }
    }
}
