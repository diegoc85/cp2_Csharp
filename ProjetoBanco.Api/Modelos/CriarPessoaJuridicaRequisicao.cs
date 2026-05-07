namespace ProjetoBanco.Api.Modelos;

public record CriarPessoaJuridicaRequisicao(
    string Nome,
    string Email,
    string Telefone,
    int AgenciaId,
    string Cnpj,
    string RazaoSocial,
    decimal FaturamentoMensal);
