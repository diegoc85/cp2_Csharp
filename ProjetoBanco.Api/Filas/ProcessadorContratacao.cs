using Microsoft.EntityFrameworkCore;
using ProjetoBanco.Api.Dados;
using ProjetoBanco.Api.Dominio;
using ProjetoBanco.Api.Servicos;

namespace ProjetoBanco.Api.Filas;

public class ProcessadorContratacao(
    IServiceScopeFactory scopeFactory,
    IFilaContratacao fila,
    ILogger<ProcessadorContratacao> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var contratacaoId = await fila.ReceberAsync(stoppingToken);
                if (contratacaoId.HasValue)
                {
                    await ProcessarAsync(contratacaoId.Value, stoppingToken);
                }
                else
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar contratacao.");
                await Task.Delay(2000, stoppingToken);
            }
        }
    }

    private async Task ProcessarAsync(int contratacaoId, CancellationToken cancellationToken)
    {
        using var escopo = scopeFactory.CreateScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<BancoContexto>();
        var servicoEmprestimo = escopo.ServiceProvider.GetRequiredService<ServicoEmprestimo>();

        var contratacao = await contexto.Contratacoes
            .Include(c => c.Cliente)
            .Include(c => c.Produto)
            .FirstOrDefaultAsync(c => c.Id == contratacaoId, cancellationToken);

        if (contratacao?.Cliente is null)
        {
            return;
        }

        var resultado = servicoEmprestimo.Analisar(
            contratacao.Cliente,
            contratacao.ValorSolicitado,
            contratacao.PrazoMeses);

        contratacao.ScoreCalculado = resultado.Score;
        contratacao.TaxaJurosMensal = resultado.TaxaJurosMensal;
        contratacao.ValorParcela = resultado.ValorParcela;
        contratacao.Mensagem = resultado.Mensagem;
        contratacao.Status = resultado.Aprovado ? StatusContratacao.Aprovada : StatusContratacao.Reprovada;

        await contexto.SaveChangesAsync(cancellationToken);
    }
}
