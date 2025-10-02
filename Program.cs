using MINIMAL_API.Dominio.DTOs;
using System.Diagnostics.Eventing.Reader;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/login", (LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "admin" && loginDTO.Senha == "password")
        return Results.Ok("Login com sucesso");
    else return Results.Unauthorized();
});

app.Run();
