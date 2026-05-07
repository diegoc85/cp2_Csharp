using System.Text;
using RabbitMQ.Client;

namespace ProjetoBanco.Api.Filas;

public class FilaContratacaoRabbitMq(IConfiguration configuration) : IFilaContratacao, IDisposable
{
    private const string NomeFila = "contratacoes-emprestimo";
    private IConnection? _conexao;
    private IModel? _canal;

    public Task PublicarAsync(int contratacaoId, CancellationToken cancellationToken = default)
    {
        var canal = ObterCanal();
        var corpo = Encoding.UTF8.GetBytes(contratacaoId.ToString());

        canal.BasicPublish(
            exchange: string.Empty,
            routingKey: NomeFila,
            basicProperties: null,
            body: corpo);

        return Task.CompletedTask;
    }

    public Task<int?> ReceberAsync(CancellationToken cancellationToken = default)
    {
        var canal = ObterCanal();
        var resultado = canal.BasicGet(NomeFila, autoAck: true);

        if (resultado is null)
        {
            return Task.FromResult<int?>(null);
        }

        var texto = Encoding.UTF8.GetString(resultado.Body.ToArray());
        return Task.FromResult<int?>(int.TryParse(texto, out var id) ? id : null);
    }

    private IModel ObterCanal()
    {
        if (_canal is not null)
        {
            return _canal;
        }

        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:Host"] ?? "localhost",
            UserName = configuration["RabbitMq:Usuario"] ?? "guest",
            Password = configuration["RabbitMq:Senha"] ?? "guest",
            Port = int.TryParse(configuration["RabbitMq:Porta"], out var porta) ? porta : 5672
        };

        _conexao = factory.CreateConnection();
        _canal = _conexao.CreateModel();
        _canal.QueueDeclare(NomeFila, durable: true, exclusive: false, autoDelete: false);

        return _canal;
    }

    public void Dispose()
    {
        _canal?.Dispose();
        _conexao?.Dispose();
    }
}
