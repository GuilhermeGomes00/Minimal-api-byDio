namespace minimal_api_DIO.Dominio.ModelViews;

public struct Home
{
    // Apenas para verificar se funcionou!
    public string? FOI { get => "FOI?"; }
    public string Mensagem { get => "Bem-Vindo a API de veículos - Minimal API"; }
    public string Doc {get => "/swagger";}
}