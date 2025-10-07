// ===== Usings necessários =====
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MINIMAL_API.Dominio.DTOs;
using MINIMAL_API.Dominio.Entidades;
using MINIMAL_API.Dominio.Interfaces;
using MINIMAL_API.Dominio.Service;
using MINIMAL_API.Enums;
using MINIMAL_API.Infraestrutura.Db;
using MINIMAL_API.Validator;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// ===== Criar o builder da aplicação =====
var builder = WebApplication.CreateBuilder(args);

// ===== Configurar JWT =====
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
var keyBytes = Encoding.ASCII.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

builder.Services.AddAuthorization();

// ===== Registrar serviços =====
builder.Services.AddScoped<IAdministrador, AdministradorService>();
builder.Services.AddScoped<IVeiculo, VeiculoService>();
builder.Services.AddScoped<VeiculoValidador>();
builder.Services.AddScoped<AdministradorValidator>();

// ===== Swagger com suporte a JWT =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT aqui"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ===== Registrar DbContext =====
builder.Services.AddDbContext<DbContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// ===== Cria o usuario master do sistema =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DbContexto>();

    if (!db.Administradores.Any(a => a.Email == "admin@mail.com"))
    {
        var admin = new Administrador
        {
            Nome = "Admin",
            Email = "admin@mail.com",
            Senha = BCrypt.Net.BCrypt.HashPassword("Admin"),
            Perfil = Perfil.ADMIN,
            DataCriacao = DateTime.UtcNow
        };

        db.Administradores.Add(admin);
        db.SaveChanges();
        Console.WriteLine("Usuário master criado com sucesso!");
    }
    else
    {
        Console.WriteLine("Usuário master já existe, não foi recriado.");
    }
}

// ===== Teste de conexão com o banco =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DbContexto>();
    try
    {
        Console.WriteLine(db.Database.CanConnect() ? "Conexão com o banco OK!" : "Não foi possível conectar ao banco.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro na conexão: {ex.Message}");
    }
}

// ===== Pipeline =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ===== Endpoints =====

// --- Login e geração de token JWT ---
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO,IAdministrador AdministradorService) =>
{
    try
    {
        // 🔍 Validação inicial
        if (loginDTO == null || string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Senha))
            return Results.BadRequest("E-mail e senha são obrigatórios.");

        // 🔎 Busca o administrador pelo e-mail
        var admin = AdministradorService.BuscaPorEmail(loginDTO.Email);

        if (admin == null)
            return Results.Unauthorized();

        // 🔐 Verifica a senha com BCrypt
        bool senhaValida = BCrypt.Net.BCrypt.Verify(loginDTO.Senha, admin.Senha);

        if (!senhaValida)
            return Results.Unauthorized();

        // 🔑 Criação do token JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtSettings.Key); // UTF8 é mais seguro para chaves longas

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Nome),
                new Claim(ClaimTypes.Role, admin.Perfil.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpireMinutes),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // ✅ Retorna o token + informações do usuário logado
        return Results.Ok(new
        {
            Token = tokenString,
            ExpiraEm = tokenDescriptor.Expires,
            Usuario = new
            {
                admin.Nome,
                admin.Email,
                Perfil = admin.Perfil.ToString()
            }
        });
    }
    catch (KeyNotFoundException)
    {
        return Results.Unauthorized(); // caso o email não exista
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro ao realizar login: {ex.Message}");
    }
});

// --- Criar administrador ---
app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministrador AdministradorService) =>
{
    try
    {
        if (!Enum.TryParse<Perfil>(administradorDTO.Perfil, true, out var perfilEnum))
            return Results.BadRequest("Perfil inválido. Valores permitidos: ADMIN,USER.");

        // 🔐 Criptografa a senha antes de salvar
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(administradorDTO.Senha);

        var administrador = new Administrador
        {
            Nome = administradorDTO.Nome,
            Email = administradorDTO.Email,
            Senha = senhaHash, // <-- senha criptografada
            Perfil = perfilEnum
        };

        AdministradorService.SalvarAdministrador(administrador);

        var administradorResponseDTO = new AdministradorResponseDTO
        {
            Nome = administrador.Nome,
            Email = administrador.Email,
            Perfil = administrador.Perfil.ToString()
        };

        return Results.Created($"/administradores/{administrador.Id}", administradorResponseDTO);
    }
    catch (DuplicateWaitObjectException ex) { return Results.Conflict(ex.Message); }
    catch (ArgumentException ex) { return Results.BadRequest(ex.Message); }
    catch (Exception ex) { return Results.Problem($"Erro ao cadastrar administrador: {ex.Message}"); }
}).RequireAuthorization();

// --- Buscar administrador por ID ---
app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministrador AdministradorService) =>
{
    try
    {
        var admin = AdministradorService.BuscaPorID(id);
        var dto = new AdministradorResponseDTO
        {
            Nome = admin.Nome,
            Email = admin.Email,
            Perfil = admin.Perfil.ToString()
        };
        return Results.Ok(dto);
    }
    catch (KeyNotFoundException ex) { return Results.NotFound(ex.Message); }
    catch (Exception ex) { return Results.Problem($"Erro ao buscar administrador: {ex.Message}"); }
}).RequireAuthorization();

// --- Listar administradores com paginação e total ---
app.MapGet("/administradores", (int pagina, string? nome, string? perfil, IAdministrador AdministradorService) =>
{
    pagina = pagina < 1 ? 1 : pagina;
    var query = AdministradorService.Todos(pagina,nome, perfil);
    var total = query.Count();
    var items = query.Skip((pagina - 1) * 10).Take(10).ToList();

    var dto = items.Select(a => new AdministradorResponseDTO
    {
        Nome = a.Nome,
        Email = a.Email,
        Perfil = a.Perfil.ToString()
    }).ToList();

    return Results.Ok(new { Total = total, Itens = dto });
}).RequireAuthorization();

// ===== Veículos =====

// --- Criar veículo ---
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
        return Results.Created(urlCompleta, veiculo);
    }
    catch (DuplicateWaitObjectException ex) { return Results.Conflict(ex.Message); }
    catch (ArgumentException ex) { return Results.BadRequest(ex.Message); }
    catch (Exception ex) { return Results.Problem($"Erro ao cadastrar veículo: {ex.Message}"); }
}).RequireAuthorization();

// --- Listar veículos com paginação ---
app.MapGet("/veiculos", (int pagina, string? nome, string? marca, IVeiculo VeiculoService) =>
{
    pagina = pagina < 1 ? 1 : pagina;
    var veiculos = VeiculoService.Todos(pagina, nome, marca);
    var veiculosDTO = veiculos.Select(v => new VeiculoDTO
    {
        Nome = v.Nome,
        Marca = v.Marca,
        Data = v.Data.ToString("yyyy-MM-dd")
    }).ToList();

    return Results.Ok(veiculosDTO);
}).RequireAuthorization();

// --- Buscar veículo por ID ---
app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculo VeiculoService) =>
{
    try
    {
        var veiculo = VeiculoService.BuscaPorID(id);
        var veiculoDTO = new VeiculoDTO
        {
            Nome = veiculo.Nome,
            Marca = veiculo.Marca,
            Data = veiculo.Data.ToString("yyyy-MM-dd")
        };
        return Results.Ok(veiculoDTO);
    }
    catch (KeyNotFoundException ex) { return Results.NotFound(ex.Message); }
    catch (Exception ex) { return Results.Problem($"Erro ao buscar veículo: {ex.Message}"); }
}).RequireAuthorization();

// --- Atualizar veículo ---
app.MapPut("/veiculos/{id}", (int id, VeiculoDTO veiculoDTO, IVeiculo VeiculoService) =>
{
    try
    {
        var veiculo = VeiculoService.BuscaPorID(id);
        veiculo.Nome = veiculoDTO.Nome;
        veiculo.Marca = veiculoDTO.Marca;

        if (!DateOnly.TryParse(veiculoDTO.Data, out var dataConvertida))
            return Results.BadRequest("Data inválida. Use o formato yyyy-MM-dd.");

        veiculo.Data = dataConvertida;
        VeiculoService.AtualizarVeiculo(veiculo);
        return Results.Ok(veiculo);
    }
    catch (DuplicateWaitObjectException ex) { return Results.Conflict(ex.Message); }
    catch (KeyNotFoundException ex) { return Results.NotFound(ex.Message); }
    catch (Exception ex) { return Results.Problem($"Erro ao atualizar veículo: {ex.Message}"); }
}).RequireAuthorization();

// --- Deletar veículo ---
app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculo VeiculoService) =>
{
    try
    {
        VeiculoService.DeletarVeiculo(id);
        return Results.NoContent();
    }
    catch (ArgumentException ex) { return Results.NotFound(ex.Message); }
    catch (Exception ex) { return Results.Problem($"Erro ao deletar veículo: {ex.Message}"); }
}).RequireAuthorization();

// ===== Executar a aplicação =====
app.UseAuthentication();
app.UseAuthorization();
app.Run();
