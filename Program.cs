using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MINIMAL_API.Dominio.DTOs;
using MINIMAL_API.Dominio.Interfaces;
using MINIMAL_API.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministrador, AdministradorService>();

// ===== Registrar serviços antes do Build =====
builder.Services.AddDbContext<DbContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ===== Criar app =====
var app = builder.Build();

// ===== Teste de conexão imediato =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DbContexto>();
    try
    {
        if (db.Database.CanConnect())
        {
            Console.WriteLine("Conexão com o banco OK!");
        }
        else
        {
            Console.WriteLine("Não foi possível conectar ao banco.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro na conexão: {ex.Message}");
    }
}
// ============================================

// ===== Configure o pipeline =====
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ===== Endpoints =====
app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministrador AdministradorService) =>
{
    if (AdministradorService.login(loginDTO) != null)
        return Results.Ok("Login com sucesso");
    else
        return Results.Unauthorized();
});

app.MapGet("/teste-db", async (DbContexto db) =>
{
    if (await db.Database.CanConnectAsync())
        return Results.Ok("Conexão com o banco OK!");
    else
        return Results.Problem("Não foi possível conectar ao banco.");
});

// ===== Rodar app =====
app.Run();
