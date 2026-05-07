namespace ProjetoBanco.Api.Modelos;

public record CriarPessoaFisicaRequisicao(
    string Nome,
    string Email,
    string Telefone,
    int AgenciaId,
    string Cpf,
    DateTime DataNascimento,
    decimal RendaMensal);
