namespace ProjetoBanco.Api.Dominio;

public abstract class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public List<Contratacao> Contratacoes { get; set; } = [];
}
