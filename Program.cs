// ===== Usings necessários =====
using Microsoft.AspNetCore.Mvc;            // Suporte a atributos como [FromBody], [HttpGet], etc.
using Microsoft.EntityFrameworkCore;       // EF Core (ORM para acesso ao banco)
using MINIMAL_API.Dominio.DTOs;            // Importa os DTOs (Data Transfer Objects)
using MINIMAL_API.Dominio.Entidades;
using MINIMAL_API.Dominio.Interfaces;      // Interfaces dos serviços
using MINIMAL_API.Dominio.Service;         // Implementações dos serviços
using MINIMAL_API.Infraestrutura.Db;
using MINIMAL_API.Validator;

// ===== Criar o builder da aplicação =====
var builder = WebApplication.CreateBuilder(args);

// ===== Registrar serviços (injeção de dependência) =====
builder.Services.AddScoped<IAdministrador, AdministradorService>();  // Sempre que for pedido IAdministrador → usa AdministradorService
builder.Services.AddScoped<IVeiculo, VeiculoService>();              // Sempre que for pedido IVeiculo → usa VeiculoService
builder.Services.AddScoped<VeiculoValidador>();

// ===== Configuração de documentação (Swagger/OpenAPI) =====
builder.Services.AddEndpointsApiExplorer(); // Necessário para mapear os endpoints minimalistas
builder.Services.AddSwaggerGen();           // Gera a documentação Swagger

// ===== Registrar DbContext e configurar SQL Server =====
builder.Services.AddDbContext<DbContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
// Aqui o EF Core é configurado para usar o SQL Server, lendo a connection string do appsettings.json

// ===== Criar o app =====
var app = builder.Build();

// ===== Teste de conexão imediato com o banco =====
// Isso serve apenas para verificar no console se a API conseguiu se conectar ao banco logo ao iniciar
using (var scope = app.Services.CreateScope()) // Cria um escopo de serviços
{
    var db = scope.ServiceProvider.GetRequiredService<DbContexto>(); // Pega o DbContexto
    try
    {
        if (db.Database.CanConnect()) // Verifica se consegue conectar
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

// ===== Configurar o pipeline =====
// Se estiver em ambiente de desenvolvimento, habilita suporte a OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ===== Definição dos Endpoints =====

// Endpoint POST /login → faz autenticação do administrador
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministrador AdministradorService) =>
{
    if (AdministradorService.login(loginDTO) != null)
        return Results.Ok("Login com sucesso");   // Retorna 200 OK se login válido
    else
        return Results.Unauthorized();            // Retorna 401 Unauthorized se inválido
});

// Endpoint GET /teste-db → verifica conexão com o banco via EF Core
app.MapGet("/teste-db", async (DbContexto db) =>
{
    if (await db.Database.CanConnectAsync())      // Verifica conexão de forma assíncrona
        return Results.Ok("Conexão com o banco OK!");
    else
        return Results.Problem("Não foi possível conectar ao banco."); // Retorna erro 500
});

// Endpoint POST /veiculos → cadastra um novo veículo
app.MapPost("/veiculos", (HttpRequest request, VeiculoDTO veiculoDTO, IVeiculo VeiculoService) =>
{
    if (!DateOnly.TryParse(veiculoDTO.Data, out var dataVeiculo))
        return Results.BadRequest("Data do veículo inválida. Use o formato YYYY-MM-DD.");
    try
    {
        var veiculo = new Veiculo
        {
            Nome = veiculoDTO.Nome,
            Marca = veiculoDTO.Marca,
            Data = dataVeiculo
        };

        VeiculoService.SalvarVeiculo(veiculo);

        var urlCompleta = $"{request.Scheme}://{request.Host}/veiculos/{veiculo.Id}";
        return Results.Created(urlCompleta, null);
    }
    catch (DuplicateWaitObjectException ex)
    {
        return Results.Conflict(ex.Message); // captura conflito de dados
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message); // captura validação
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro ao cadastrar veículo: {ex.Message}"); // captura erro inesperado
    }
});

app.MapGet("/veiculos/{id}", (int id, IVeiculo veiculoService) =>
{
    var veiculo = veiculoService.BuscaPorID(id);
    if (veiculo == null)
        return Results.NotFound();

    var veiculoDTO = new VeiculoDTO
    {
        Nome = veiculo.Nome,
        Marca = veiculo.Marca,
        Data = veiculo.Data.ToString("dd-MM-yyyy")
    };

    return Results.Ok(veiculoDTO); // 200 OK com o DTO
});

app.MapDelete("/veiculos/{id}", (int id, IVeiculo veiculoService) =>
{
    try
    {
        veiculoService.DeletarVeiculo(id);
        return Results.NoContent();
    }
    catch (ArgumentException ex)
    {
        return Results.NotFound(ex.Message); // captura erro de não encontrado
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro ao deletar veículo: {ex.Message}"); // captura erro inesperado
    }
});



// ===== Habilitar Swagger =====
app.UseSwagger();    // Disponibiliza o endpoint JSON do Swagger
app.UseSwaggerUI();  // Disponibiliza a interface web interativa do Swagger

// ===== Executar a aplicação =====
app.Run();
