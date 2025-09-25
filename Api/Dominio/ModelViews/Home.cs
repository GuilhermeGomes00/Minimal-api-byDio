namespace minimal_api_DIO.Dominio.ModelViews;

public struct Home
{
    // Apenas para verificar se funcionou!
    public string Mensagem { get => "Bem-Vindo a API de veículos - Minimal API"; }
    public String Instruções
    {
        get => "Para ter acesso total, use o Email: 'Adm@teste.com' com a senha '123456', use o Token para não ter conflito" +
               " de autorização.";
    }
    
    public string Doc {get => "Acesse o Swagger aqui:";}
    public string API
    {
        get => "/swagger";
    }
}