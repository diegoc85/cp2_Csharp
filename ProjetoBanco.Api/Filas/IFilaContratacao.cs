namespace ProjetoBanco.Api.Filas;

public interface IFilaContratacao
{
    Task PublicarAsync(int contratacaoId, CancellationToken cancellationToken = default);
    Task<int?> ReceberAsync(CancellationToken cancellationToken = default);
}
