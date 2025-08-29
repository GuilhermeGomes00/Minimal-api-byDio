using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api_DIO.Dominio.DTOs;
using minimal_api_DIO.Dominio.Servicos;
using minimal_api_DIO.Infra.Db;
using minimal_api_DIO.Infra.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminitradorServico,  AdministradorServico>(); 
// Aqui registramos no container de injeção de dependência que, sempre que alguém pedir "IAdminitradorServico", 
// será entregue uma instância de "AdministradorServico".


builder.Services.AddDbContext<DbContexto>(options => {
    options.UseSqlite(
        "Data Source=Administrador.Sqlite");
});


var app = builder.Build();



app.MapGet("/", () => "ABRIU É PQ FUNCIONOU!!");
app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminitradorServico administradorServico) => // *
{
    if (administradorServico.Login(loginDTO) != null)  // * 
        return Results.Ok("Login feito com sucesso!");
    else 
        return Results.Unauthorized();
});

app.Run();
