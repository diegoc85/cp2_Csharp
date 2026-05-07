using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoBanco.Api.Dados;
using ProjetoBanco.Api.Dominio;
using ProjetoBanco.Api.Filas;
using ProjetoBanco.Api.Modelos;

namespace ProjetoBanco.Api.Controllers;

[ApiController]
[Route("api/contratacoes")]
public class ContratacoesController(BancoContexto contexto, IFilaContratacao fila) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Solicitar(SolicitarContratacaoRequisicao requisicao)
    {
        if (await contexto.Clientes.FindAsync(requisicao.ClienteId) is null)
        {
            return NotFound(new ErroResposta("Cliente nao encontrado."));
        }

        var emprestimo = await contexto.Emprestimos.FirstOrDefaultAsync();
        if (emprestimo is null)
        {
            emprestimo = new Emprestimo
            {
                Nome = "Emprestimo pessoal",
                ValorMinimo = 500,
                ValorMaximo = 50000,
                PrazoMaximoMeses = 48
            };
            contexto.Emprestimos.Add(emprestimo);
            await contexto.SaveChangesAsync();
        }

        var contratacao = new Contratacao
        {
            ClienteId = requisicao.ClienteId,
            ProdutoId = emprestimo.Id,
            ValorSolicitado = requisicao.ValorSolicitado,
            PrazoMeses = requisicao.PrazoMeses,
            Status = StatusContratacao.Pendente,
            Mensagem = "Contratacao recebida e enviada para analise."
        };

        contexto.Contratacoes.Add(contratacao);
        await contexto.SaveChangesAsync();

        await fila.PublicarAsync(contratacao.Id);

        return AcceptedAtAction(nameof(BuscarPorId), new { id = contratacao.Id }, contratacao);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var contratacao = await contexto.Contratacoes
            .Include(c => c.Cliente)
            .Include(c => c.Produto)
            .FirstOrDefaultAsync(c => c.Id == id);

        return contratacao is null ? NotFound(new ErroResposta("Contratacao nao encontrada.")) : Ok(contratacao);
    }
}
