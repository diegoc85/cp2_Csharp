namespace ProjetoBanco.Api.Dominio;

public class PessoaFisica : Cliente
{
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public decimal RendaMensal { get; set; }
}
