using Microsoft.EntityFrameworkCore;
using nom_ws_datosempleado_be;
using nom_ws_datosempleado_be.Data;

var builder = WebApplication.CreateBuilder(args);

var sshTunnel = new SshTunnelService();
// Intentar conectar el túnel SSH, pero no detener el arranque si falla
try
{
    sshTunnel.Connect(builder.Configuration);
}
catch (Exception ex)
{
    Console.WriteLine($"Error iniciando túnel SSH en startup: {ex.Message}");
}
builder.Services.AddSingleton(sshTunnel);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{

    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"
        )
     );
});
// Add services to the container.

builder.Services.AddControllers();
// CORS: permitir orígenes, cabeceras y métodos necesarios durante desarrollo
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    );
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Asegurar carpeta de logs para stdout del host IIS
try
{
    var logDir = System.IO.Path.Combine(AppContext.BaseDirectory, "logs");
    if (!System.IO.Directory.Exists(logDir))
        System.IO.Directory.CreateDirectory(logDir);
}
catch { }

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}



app.UseHttpsRedirection();

// CORS middleware (debe ir antes de autenticación/autorización)
app.UseCors("AllowAll");

// JWT middleware: extrae id_usuario del token y lo adjunta al contexto
app.UseMiddleware<nom_ws_datosempleado_be.Middleware.JwtMiddleware>();

app.UseAuthorization();

app.Lifetime.ApplicationStopped.Register(() =>
{
    sshTunnel.Dispose();
});


app.MapControllers();

app.Run();
