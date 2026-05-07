namespace ProjetoBanco.Api.Dominio;

public abstract class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public int AgenciaId { get; set; }
    public Agencia? Agencia { get; set; }
    public List<Contratacao> Contratacoes { get; set; } = [];
}
