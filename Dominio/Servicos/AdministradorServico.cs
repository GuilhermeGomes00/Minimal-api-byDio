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
}