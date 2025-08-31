using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api_DIO.Dominio.DTOs;
using minimal_api_DIO.Dominio.Entidades;
using minimal_api_DIO.Dominio.ModelViews;
using minimal_api_DIO.Dominio.Servicos;
using minimal_api_DIO.Infra.Db;
using minimal_api_DIO.Infra.Interfaces;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminitradorServico,  AdministradorServico>(); 
// Aqui registramos no container de injeção de dependência que, sempre que alguém pedir "IAdminitradorServico", 
// será entregue uma instância de "AdministradorServico".
builder.Services.AddScoped<IVeiculosServico, VeiculosServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DbContexto>(options => {
    options.UseSqlite(
        "Data Source=Administrador.Sqlite");
});

var app = builder.Build();
#endregion

#region API

    #region Home
app.MapGet("/", () => Results.Json(new Home()));
    #endregion

    #region adm
app.MapPost("/administrador/login", ([FromBody] LoginDTO loginDTO, IAdminitradorServico administradorServico) => // *
{
    if (administradorServico.Login(loginDTO) != null)  // * 
        return Results.Ok("Login feito com sucesso!");
    else 
        return Results.Unauthorized();
});
    #endregion

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculosServico veiculosServico) =>
{
    var veiculo = new Veiculos
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
        veiculosServico.Incluir(veiculo);
        
        return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
    });

#endregion
#region app
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion