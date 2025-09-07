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
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
    #endregion

    #region adm
app.MapPost("/administrador/login", ([FromBody] LoginDTO loginDTO, IAdminitradorServico administradorServico) => // *
{
    if (administradorServico.Login(loginDTO) != null)  // * 
        return Results.Ok("Login feito com sucesso!");
    else 
        return Results.Unauthorized();
}).WithTags("Administrador");
    #endregion

    #region Veiculos
    
 ErroDeValidacao ValidaDTO(VeiculoDTO veiculoDTO){
        var validacao = new ErroDeValidacao{
            Mensagens = new  List<String>()
        };
        
        if(string.IsNullOrEmpty(veiculoDTO.Nome))
            validacao.Mensagens.Add("O nome não pode ser vazio!");
        
        if(string.IsNullOrEmpty(veiculoDTO.Marca))
            validacao.Mensagens.Add("A marca não pode estar em branco!");
        
        if(veiculoDTO.Ano < 1950)
            validacao.Mensagens.Add("Ano do carro indisponivel");
        
        return validacao;
    }


// Post
    app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculosServico veiculosServico) =>
    {
        var validacao = ValidaDTO(veiculoDTO);
        if (validacao.Mensagens.Count > 0)
            return Results.BadRequest(validacao.Mensagens);
        
        var veiculo = new Veiculos
        {
            Nome = veiculoDTO.Nome,
            Marca = veiculoDTO.Marca,
            Ano = veiculoDTO.Ano
        };
        veiculosServico.Incluir(veiculo);
        
        return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
    }).WithTags("Veiculos");

// Get
    app.MapGet("/veiculos", ([FromQuery]int? pagina, IVeiculosServico veiculosServico) =>
    {
        var veiculos = veiculosServico.Todos(pagina);

        return Results.Ok(veiculos);
    }).WithTags("Veiculos");

    app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculosServico veiculosServico) =>
    {
        var veiculo = veiculosServico.GetPorId(id);

        if (veiculo == null) return Results.NotFound();
        
        return Results.Ok(veiculo);
    }).WithTags("Veiculos");

// Put
    app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculosServico veiculosServico) =>
    {
        var veiculo = veiculosServico.GetPorId(id);

        if (veiculo == null) return Results.NotFound();
        
        var validacao = ValidaDTO(veiculoDTO);
        if (validacao.Mensagens.Count > 0)
            return Results.BadRequest(validacao.Mensagens);
        
        veiculo.Nome = veiculoDTO.Nome;
        veiculo.Marca = veiculoDTO.Marca;
        veiculo.Ano = veiculoDTO.Ano;
        
        veiculosServico.Atualizar(veiculo);
        
        return Results.Ok(veiculo);
    }).WithTags("Veiculos");

// Delete
    app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculosServico veiculosServico) =>
    {
        var veiculo = veiculosServico.GetPorId(id);
        if (veiculo == null) return Results.NotFound();
        
        veiculosServico.Apagar(veiculo);
        
        return Results.NoContent();
    }).WithTags("Veiculos");
    #endregion

#endregion
#region app
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion