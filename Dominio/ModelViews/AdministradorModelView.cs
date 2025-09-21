using minimal_api_DIO.Dominio.Enums;

namespace minimal_api_DIO.Dominio.ModelViews;

public record AdministradorModelView
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Perfil { get; set; }
}