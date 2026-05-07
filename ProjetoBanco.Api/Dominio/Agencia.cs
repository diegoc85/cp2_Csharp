namespace ProjetoBanco.Api.Dominio;

public class Agencia
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public List<Cliente> Clientes { get; set; } = [];
}
