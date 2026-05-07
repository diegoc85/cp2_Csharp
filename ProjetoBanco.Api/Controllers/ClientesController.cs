using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoBanco.Api.Dados;
using ProjetoBanco.Api.Dominio;
using ProjetoBanco.Api.Modelos;

namespace ProjetoBanco.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController(BancoContexto contexto) : ControllerBase
{
    [HttpPost("pf")]
    public async Task<IActionResult> CriarPessoaFisica(CriarPessoaFisicaRequisicao requisicao)
    {
        if (await contexto.Agencias.FindAsync(requisicao.AgenciaId) is null)
        {
            return NotFound(new ErroResposta("Agencia nao encontrada."));
        }

        if (await contexto.PessoasFisicas.CountAsync(p => p.Cpf == requisicao.Cpf) > 0)
        {
            return Conflict(new ErroResposta("CPF ja cadastrado."));
        }

        var pessoaFisica = new PessoaFisica
        {
            Nome = requisicao.Nome,
            Email = requisicao.Email,
            Telefone = requisicao.Telefone,
            AgenciaId = requisicao.AgenciaId,
            Cpf = requisicao.Cpf,
            DataNascimento = requisicao.DataNascimento,
            RendaMensal = requisicao.RendaMensal
        };

        contexto.PessoasFisicas.Add(pessoaFisica);
        await contexto.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = pessoaFisica.Id }, pessoaFisica);
    }

    [HttpPost("pj")]
    public async Task<IActionResult> CriarPessoaJuridica(CriarPessoaJuridicaRequisicao requisicao)
    {
        if (await contexto.Agencias.FindAsync(requisicao.AgenciaId) is null)
        {
            return NotFound(new ErroResposta("Agencia nao encontrada."));
        }

        if (await contexto.PessoasJuridicas.CountAsync(p => p.Cnpj == requisicao.Cnpj) > 0)
        {
            return Conflict(new ErroResposta("CNPJ ja cadastrado."));
        }

        var pessoaJuridica = new PessoaJuridica
        {
            Nome = requisicao.Nome,
            Email = requisicao.Email,
            Telefone = requisicao.Telefone,
            AgenciaId = requisicao.AgenciaId,
            Cnpj = requisicao.Cnpj,
            RazaoSocial = requisicao.RazaoSocial,
            FaturamentoMensal = requisicao.FaturamentoMensal
        };

        contexto.PessoasJuridicas.Add(pessoaJuridica);
        await contexto.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = pessoaJuridica.Id }, pessoaJuridica);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var cliente = await contexto.Clientes
            .Include(c => c.Agencia)
            .FirstOrDefaultAsync(c => c.Id == id);

        return cliente is null ? NotFound(new ErroResposta("Cliente nao encontrado.")) : Ok(cliente);
    }
}
