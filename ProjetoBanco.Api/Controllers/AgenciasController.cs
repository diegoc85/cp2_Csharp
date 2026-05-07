using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoBanco.Api.Dados;
using ProjetoBanco.Api.Dominio;
using ProjetoBanco.Api.Modelos;

namespace ProjetoBanco.Api.Controllers;

[ApiController]
[Route("api/agencias")]
public class AgenciasController(BancoContexto contexto) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Criar(CriarAgenciaRequisicao requisicao)
    {
        if (await contexto.Agencias.CountAsync(a => a.Numero == requisicao.Numero) > 0)
        {
            return Conflict(new ErroResposta("Ja existe agencia com este numero."));
        }

        var agencia = new Agencia
        {
            Numero = requisicao.Numero,
            Nome = requisicao.Nome,
            Endereco = requisicao.Endereco
        };

        contexto.Agencias.Add(agencia);
        await contexto.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = agencia.Id }, agencia);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var agencia = await contexto.Agencias.FindAsync(id);
        return agencia is null ? NotFound(new ErroResposta("Agencia nao encontrada.")) : Ok(agencia);
    }
}
