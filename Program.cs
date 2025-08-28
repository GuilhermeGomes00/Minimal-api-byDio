using Microsoft.EntityFrameworkCore;
using minimal_api_DIO.Dominio.DTOs;
using minimal_api_DIO.Infra.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContexto>(options => {
    options.UseSqlite(
        "Data Source=Administrador.Sqlite");
});

var app = builder.Build();



app.MapGet("/teste", () => "Hello World!");
app.MapPost("/login", (LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "Adm@teste.com" && loginDTO.Senha == "123456") 
        return Results.Ok("Login feito com sucesso!");
    else 
        return Results.Unauthorized();
});

app.Run();
