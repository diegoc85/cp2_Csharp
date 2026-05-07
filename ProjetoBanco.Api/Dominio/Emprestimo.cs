namespace ProjetoBanco.Api.Dominio;

public class Emprestimo : Produto
{
    public decimal ValorMinimo { get; set; }
    public decimal ValorMaximo { get; set; }
    public int PrazoMaximoMeses { get; set; }
}
