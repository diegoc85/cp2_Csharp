namespace ProjetoBanco.Api.Dominio;

public class Contratacao
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }
    public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
    public StatusContratacao Status { get; set; } = StatusContratacao.Pendente;
    public decimal ValorSolicitado { get; set; }
    public int PrazoMeses { get; set; }
    public int ScoreCalculado { get; set; }
    public decimal TaxaJurosMensal { get; set; }
    public decimal ValorParcela { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}
