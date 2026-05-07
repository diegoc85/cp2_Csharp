namespace ProjetoBanco.Api.Servicos;

public record ResultadoEmprestimo(
    bool Aprovado,
    int Score,
    decimal TaxaJurosMensal,
    decimal ValorParcela,
    string Mensagem);
