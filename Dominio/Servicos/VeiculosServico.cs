using Microsoft.EntityFrameworkCore;
using minimal_api_DIO.Dominio.Entidades;
using minimal_api_DIO.Infra.Db;
using minimal_api_DIO.Infra.Interfaces;

namespace minimal_api_DIO.Dominio.Servicos;

public class VeiculosServico : IVeiculosServico
{
    private readonly DbContexto _ctx;
    public VeiculosServico(DbContexto ctx)
    {
        _ctx = ctx;
    }
    
    public List<Veiculos> Todos(int pagina = 1, string? nome = null, string? marca = null)
    {
        var query = _ctx
            .Veiculos
            .AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query
                .Where(
                    v => EF.Functions.Like
                    (
                        v.Nome.ToLower(),$"%{nome}%")
                    );
        }
        
        
        
        return query.ToList();

    }

    public Veiculos? GetPorId(int id)
    {
        return _ctx.Veiculos.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Incluir(Veiculos veiculos)
    {
        _ctx.Veiculos.Add(veiculos);
        _ctx.SaveChanges();
    }

    public void Atualizar(Veiculos veiculos)
    {
        _ctx.Veiculos.Update(veiculos);
        _ctx.SaveChanges();
    }

    public void Apagar(Veiculos veiculos)
    {
        _ctx.Veiculos.Remove(veiculos);
        _ctx.SaveChanges();
    }
}