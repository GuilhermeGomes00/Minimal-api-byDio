using minimal_api_DIO.Dominio.DTOs;
using minimal_api_DIO.Dominio.Entidades;

namespace minimal_api_DIO.Infra.Interfaces;

public interface IAdminitradorServico // É a interface que define o contrato (o que precisa existir em qualquer classe que a implemente).
{
    Administrador? Login(LoginDTO loginDTO); // Método do contrato que deve ser implementado para fazer login.
    Administrador Incluir(Administrador administrador);
    Administrador? GetPorId (int id);
    List<Administrador> Todos(int? pagina);
}