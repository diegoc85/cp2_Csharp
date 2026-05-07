using ProjetoBanco.Api.Dominio;

namespace ProjetoBanco.Api.Servicos;

public class ServicoScore
{
    public int Calcular(Cliente cliente)
    {
        var score = cliente switch
        {
            PessoaFisica pessoaFisica => CalcularPessoaFisica(pessoaFisica),
            PessoaJuridica pessoaJuridica => CalcularPessoaJuridica(pessoaJuridica),
            _ => 300
        };

        return Math.Clamp(score, 0, 1000);
    }

    private static int CalcularPessoaFisica(PessoaFisica pessoaFisica)
    {
        var idade = DateTime.Today.Year - pessoaFisica.DataNascimento.Year;
        if (pessoaFisica.DataNascimento.Date > DateTime.Today.AddYears(-idade))
        {
            idade--;
        }

        var score = 300;
        score += pessoaFisica.RendaMensal >= 8000 ? 300 : pessoaFisica.RendaMensal >= 4000 ? 220 : 120;
        score += idade >= 25 ? 120 : 40;
        score += pessoaFisica.Email.Contains('@') ? 50 : 0;
        score += pessoaFisica.Telefone.Length >= 10 ? 50 : 0;

        return score;
    }

    private static int CalcularPessoaJuridica(PessoaJuridica pessoaJuridica)
    {
        var score = 350;
        score += pessoaJuridica.FaturamentoMensal >= 100000 ? 350 : pessoaJuridica.FaturamentoMensal >= 30000 ? 250 : 120;
        score += pessoaJuridica.RazaoSocial.Length >= 5 ? 80 : 0;
        score += pessoaJuridica.Email.Contains('@') ? 60 : 0;
        score += pessoaJuridica.Telefone.Length >= 10 ? 40 : 0;

        return score;
    }
}
