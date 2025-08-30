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
        throw new NotImplementedException();
    }

    public Veiculos GetPorId(int id)
    {
        throw new NotImplementedException();
    }

    public async Task Setar(Veiculos veiculos)
    {
        throw new NotImplementedException();
    }

    public async Task Atualizar(Veiculos veiculos)
    {
        var veiculo = _ctx
            .Veiculos
            .FirstOrDefault(v => v.Id == veiculos.Id);
        
        
    }

    public async Task Apagar(Veiculos veiculos)
    {
        var veiculo = await _ctx
            .Veiculos
            .SingleOrDefaultAsync(v => v.Id == veiculos.Id);
        if (veiculo == null) Results.NotFound();
        await _ctx.SaveChangesAsync();
    }
}