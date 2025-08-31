using minimal_api_DIO.Dominio.DTOs;
using minimal_api_DIO.Dominio.Entidades;

namespace minimal_api_DIO.Infra.Interfaces;

public interface IVeiculosServico // É a interface que define o contrato (o que precisa existir em qualquer classe que a implemente).
{
    List<Veiculos> Todos(int pagina = 1, string? nome = null, string? marca = null);
    
    Veiculos? GetPorId(int id);
    
    void Incluir(Veiculos veiculos);
    
    void Atualizar(Veiculos veiculos);
    
    void Apagar(Veiculos veiculos);
}