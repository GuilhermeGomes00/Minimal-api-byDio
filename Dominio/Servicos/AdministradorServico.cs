using minimal_api_DIO.Dominio.DTOs;
using minimal_api_DIO.Dominio.Entidades;
using minimal_api_DIO.Infra.Db;
using minimal_api_DIO.Infra.Interfaces;

namespace minimal_api_DIO.Dominio.Servicos;

public class AdministradorServico : IAdminitradorServico
{
    private readonly DbContexto _ctx;
    public AdministradorServico(DbContexto ctx)
    {
        _ctx = ctx;
    }
    
    public Administrador? Login(LoginDTO loginDTO) // Implementação do método da interface. 
                                                  // Ele verifica se existe um administrador no banco com o email e senha informados.
    {
        var adm = _ctx
            .Administradores
            .Where(
                a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha)   
                //Lembrando que "a" ou "a.Email", o "a" se refere a "Administrador"
            .FirstOrDefault(); 

        return adm; 

    }

    public Administrador Incluir(Administrador administrador)
    {
        _ctx.Administradores.Add(administrador);
        _ctx.SaveChanges();
        
        return administrador;
    }

    public Administrador? GetPorId(int id)
    {
        return _ctx.Administradores.Where(a => a.Id == id).FirstOrDefault();
    }

    public List<Administrador> Todos(int? pagina)
    {
        var query = _ctx
            .Administradores
            .AsQueryable();
        
        int itensPorPagina = 10;
        if (pagina != null)
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        
        return query.ToList();
    }
}