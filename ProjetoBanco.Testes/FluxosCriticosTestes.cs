using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProjetoBanco.Testes;

public class FluxosCriticosTestes : IClassFixture<FabricaApi>
{
    private readonly HttpClient _cliente;

    public FluxosCriticosTestes(FabricaApi fabrica)
    {
        _cliente = fabrica.CreateClient();
    }

    [Fact]
    public async Task DeveCadastrarPessoaFisicaEImpedirCpfDuplicado()
    {
        var agenciaId = await CriarAgenciaAsync("0001");
        var requisicao = new
        {
            nome = "Ana Silva",
            email = "ana@email.com",
            telefone = "11999999999",
            agenciaId,
            cpf = "12345678901",
            dataNascimento = "1995-01-10",
            rendaMensal = 9000
        };

        var primeiraResposta = await _cliente.PostAsJsonAsync("/api/clientes/pf", requisicao);
        var segundaResposta = await _cliente.PostAsJsonAsync("/api/clientes/pf", requisicao);

        Assert.Equal(HttpStatusCode.Created, primeiraResposta.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segundaResposta.StatusCode);
    }

    [Fact]
    public async Task DeveCadastrarPessoaJuridicaEImpedirCnpjDuplicado()
    {
        var agenciaId = await CriarAgenciaAsync("0002");
        var requisicao = new
        {
            nome = "Tech Solucoes",
            email = "contato@tech.com",
            telefone = "1133334444",
            agenciaId,
            cnpj = "12345678000199",
            razaoSocial = "Tech Solucoes Ltda",
            faturamentoMensal = 120000
        };

        var primeiraResposta = await _cliente.PostAsJsonAsync("/api/clientes/pj", requisicao);
        var segundaResposta = await _cliente.PostAsJsonAsync("/api/clientes/pj", requisicao);

        Assert.Equal(HttpStatusCode.Created, primeiraResposta.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segundaResposta.StatusCode);
    }

    [Fact]
    public async Task DeveRecusarClienteComAgenciaInexistente()
    {
        var resposta = await _cliente.PostAsJsonAsync("/api/clientes/pf", new
        {
            nome = "Bruno Lima",
            email = "bruno@email.com",
            telefone = "11988887777",
            agenciaId = 999,
            cpf = "98765432100",
            dataNascimento = "1998-05-15",
            rendaMensal = 5000
        });

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task DeveSolicitarContratacaoValidaEProcessarStatus()
    {
        var clienteId = await CriarPessoaFisicaAsync("0003", "22233344455");

        var resposta = await _cliente.PostAsJsonAsync("/api/contratacoes", new
        {
            clienteId,
            valorSolicitado = 10000,
            prazoMeses = 24
        });

        Assert.Equal(HttpStatusCode.Accepted, resposta.StatusCode);

        var corpo = await LerJsonAsync(resposta);
        var contratacaoId = corpo.RootElement.GetProperty("id").GetInt32();
        var status = await AguardarStatusAsync(contratacaoId);

        Assert.Equal("Aprovada", status);
    }

    [Fact]
    public async Task DeveRecusarContratacaoParaClienteInexistente()
    {
        var resposta = await _cliente.PostAsJsonAsync("/api/contratacoes", new
        {
            clienteId = 999,
            valorSolicitado = 10000,
            prazoMeses = 24
        });

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    private async Task<int> CriarAgenciaAsync(string numero)
    {
        var resposta = await _cliente.PostAsJsonAsync("/api/agencias", new
        {
            numero,
            nome = $"Agencia {numero}",
            endereco = "Avenida Paulista, 1000"
        });

        var corpo = await LerJsonAsync(resposta);
        return corpo.RootElement.GetProperty("id").GetInt32();
    }

    private async Task<int> CriarPessoaFisicaAsync(string numeroAgencia, string cpf)
    {
        var agenciaId = await CriarAgenciaAsync(numeroAgencia);
        var resposta = await _cliente.PostAsJsonAsync("/api/clientes/pf", new
        {
            nome = "Carla Souza",
            email = "carla@email.com",
            telefone = "11977776666",
            agenciaId,
            cpf,
            dataNascimento = "1990-02-20",
            rendaMensal = 12000
        });

        var corpo = await LerJsonAsync(resposta);
        return corpo.RootElement.GetProperty("id").GetInt32();
    }

    private async Task<string> AguardarStatusAsync(int contratacaoId)
    {
        for (var tentativa = 0; tentativa < 20; tentativa++)
        {
            var resposta = await _cliente.GetAsync($"/api/contratacoes/{contratacaoId}");
            var corpo = await LerJsonAsync(resposta);
            var status = corpo.RootElement.GetProperty("status").GetString();

            if (status != "Pendente")
            {
                return status ?? string.Empty;
            }

            await Task.Delay(200);
        }

        return "Pendente";
    }

    private static async Task<JsonDocument> LerJsonAsync(HttpResponseMessage resposta)
    {
        var texto = await resposta.Content.ReadAsStringAsync();
        return JsonDocument.Parse(texto);
    }
}
