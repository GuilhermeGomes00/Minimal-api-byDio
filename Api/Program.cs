using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api_DIO.Dominio.DTOs;
using minimal_api_DIO.Dominio.Entidades;
using minimal_api_DIO.Dominio.Enums;
using minimal_api_DIO.Dominio.ModelViews;
using minimal_api_DIO.Dominio.Servicos;
using minimal_api_DIO.Infra.Db;
using minimal_api_DIO.Infra.Interfaces;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "123456";
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});
builder.Services.AddAuthorization();


builder.Services.AddScoped<IAdminitradorServico,  AdministradorServico>(); 
// Aqui registramos no container de injeção de dependência que, sempre que alguém pedir "IAdminitradorServico", 
// será entregue uma instância de "AdministradorServico".
builder.Services.AddScoped<IVeiculosServico, VeiculosServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token Jwt aqui",
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});



builder.Services.AddDbContext<DbContexto>(options => {
    options.UseSqlite(
        "Data Source=Administrador.Sqlite");
});

var app = builder.Build();
#endregion

#region API

    #region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
    #endregion

    #region adm

    string GerarTokenJwt(Administrador administrador)
    {
        if(string.IsNullOrEmpty(key)) return string.Empty;
        var securityKey =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        
        var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim("Email", administrador.Email),
            new Claim("Perfil", administrador.Perfil),
            new Claim(ClaimTypes.Role, administrador.Perfil)
            
        };
        var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credential
            );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

app.MapPost("/administrador/login", ([FromBody] LoginDTO loginDTO, IAdminitradorServico administradorServico) => // Login
{
    var adm = administradorServico.Login(loginDTO);
    if ( adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdmLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token

        });
    }
    else 
        return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administrador");

app.MapPost("/administrador",
    ([FromBody] AdministradorDTO administradorDTO, IAdminitradorServico administradorServico) => // Post para criar conta-adm
    {
        var validacao = new ErroDeValidacao
        {
            Mensagens = new List<string>()
        };

        if (string.IsNullOrEmpty(administradorDTO.Email))
        {
            validacao.Mensagens.Add("Email não pode ser vazio");
        }

        if (string.IsNullOrEmpty(administradorDTO.Senha))
        {
            validacao.Mensagens.Add("Senha não pode ser vazia");
        }

        if (administradorDTO.Perfil == null)
        {
            validacao.Mensagens.Add("Perfil não pode ser vazio");
        }


        if (validacao.Mensagens.Count > 0)
            return Results.BadRequest(validacao.Mensagens);

        
        var administrador = new Administrador
        {
            Email = administradorDTO.Email,
            Senha = administradorDTO.Senha,
            Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
        };
        administradorServico.Incluir(administrador);
        

        return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
    }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"}).WithTags("Administrador");

app.MapGet("/administradores", ([FromQuery] int? pagina, IAdminitradorServico administradorServico) => // get para ver listagem de contas-adm
{
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);
    foreach (var administrador in administradores)
    {
        adms.Add(new AdministradorModelView
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
    }
    return Results.Ok(adms);
}).RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
    .WithTags("Administrador");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdminitradorServico administradorServico) => // Get para ver infos de uma conta especifica-adm
{
    var administrador = administradorServico.GetPorId(id);

    if (administrador == null) return Results.NotFound();
        
    return Results.Ok(new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"}).WithTags("Administrador");
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
    }).RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm,Editor"}).WithTags("Veiculos");

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
    }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{Roles = "Adm,Editor"}).WithTags("Veiculos");

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
    }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"}).WithTags("Veiculos");

// Delete
    app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculosServico veiculosServico) =>
    {
        var veiculo = veiculosServico.GetPorId(id);
        if (veiculo == null) return Results.NotFound();
        
        veiculosServico.Apagar(veiculo);
        
        return Results.NoContent();
    }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"}).WithTags("Veiculos");
    #endregion

#endregion
#region app
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.Run();
#endregion