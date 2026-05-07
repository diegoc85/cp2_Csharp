using ProjetoBanco.Api.Dominio;

namespace ProjetoBanco.Api.Servicos;

public class ServicoEmprestimo(ServicoScore servicoScore)
{
    public ResultadoEmprestimo Analisar(Cliente cliente, decimal valorSolicitado, int prazoMeses)
    {
        var score = servicoScore.Calcular(cliente);
        var rendaBase = ObterRendaBase(cliente);

        if (valorSolicitado < 500)
        {
            return new ResultadoEmprestimo(false, score, 0, 0, "Valor minimo para emprestimo e 500.");
        }

        if (valorSolicitado > 50000)
        {
            return new ResultadoEmprestimo(false, score, 0, 0, "Valor maximo para emprestimo e 50000.");
        }

        if (prazoMeses < 6 || prazoMeses > 48)
        {
            return new ResultadoEmprestimo(false, score, 0, 0, "Prazo deve ficar entre 6 e 48 meses.");
        }

        if (score < 600)
        {
            return new ResultadoEmprestimo(false, score, 0, 0, "Score insuficiente para aprovacao do emprestimo.");
        }

        var taxa = score >= 850 ? 0.018m : score >= 750 ? 0.024m : 0.032m;
        var parcela = CalcularParcela(valorSolicitado, taxa, prazoMeses);
        var limiteParcela = rendaBase * 0.30m;

        if (parcela > limiteParcela)
        {
            return new ResultadoEmprestimo(false, score, taxa, parcela, "Parcela acima de 30% da renda analisada.");
        }

        return new ResultadoEmprestimo(true, score, taxa, parcela, "Emprestimo aprovado.");
    }

    private static decimal ObterRendaBase(Cliente cliente)
    {
        return cliente switch
        {
            PessoaFisica pessoaFisica => pessoaFisica.RendaMensal,
            PessoaJuridica pessoaJuridica => pessoaJuridica.FaturamentoMensal * 0.20m,
            _ => 0
        };
    }

    private static decimal CalcularParcela(decimal valor, decimal taxa, int prazoMeses)
    {
        var taxaDouble = (double)taxa;
        var fator = Math.Pow(1 + taxaDouble, prazoMeses);
        var parcela = (double)valor * taxaDouble * fator / (fator - 1);

        return Math.Round((decimal)parcela, 2);
    }
}
