using System.Threading.Channels;

namespace ProjetoBanco.Api.Filas;

public class FilaContratacaoMemoria : IFilaContratacao
{
    private readonly Channel<int> _canal = Channel.CreateUnbounded<int>();

    public async Task PublicarAsync(int contratacaoId, CancellationToken cancellationToken = default)
    {
        await _canal.Writer.WriteAsync(contratacaoId, cancellationToken);
    }

    public async Task<int?> ReceberAsync(CancellationToken cancellationToken = default)
    {
        var contratacaoId = await _canal.Reader.ReadAsync(cancellationToken);
        return contratacaoId;
    }
}
