using minimal_api_DIO.Dominio.Enums;

namespace minimal_api_DIO.Dominio.DTOs;

public record AdministradorDTO(string Email, string Senha, Perfil? Perfil);