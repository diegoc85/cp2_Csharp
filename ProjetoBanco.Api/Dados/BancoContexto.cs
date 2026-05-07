using Microsoft.EntityFrameworkCore;
using ProjetoBanco.Api.Dominio;

namespace ProjetoBanco.Api.Dados;

public class BancoContexto(DbContextOptions<BancoContexto> opcoes) : DbContext(opcoes)
{
    public DbSet<Agencia> Agencias => Set<Agencia>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<PessoaFisica> PessoasFisicas => Set<PessoaFisica>();
    public DbSet<PessoaJuridica> PessoasJuridicas => Set<PessoaJuridica>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Emprestimo> Emprestimos => Set<Emprestimo>();
    public DbSet<Contratacao> Contratacoes => Set<Contratacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agencia>().ToTable("PB_AGENCIAS");
        modelBuilder.Entity<Cliente>().ToTable("PB_CLIENTES");
        modelBuilder.Entity<Produto>().ToTable("PB_PRODUTOS");
        modelBuilder.Entity<Contratacao>().ToTable("PB_CONTRATACOES");

        modelBuilder.Entity<Cliente>()
            .HasDiscriminator<string>("TipoCliente")
            .HasValue<PessoaFisica>("PessoaFisica")
            .HasValue<PessoaJuridica>("PessoaJuridica");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Nome)
            .HasMaxLength(120);

        modelBuilder.Entity<Produto>()
            .HasDiscriminator<string>("TipoProduto")
            .HasValue<Emprestimo>("Emprestimo")
            .HasValue<MaquinaDeCartao>("MaquinaDeCartao")
            .HasValue<ReceberSalario>("ReceberSalario");

        modelBuilder.Entity<Agencia>()
            .HasIndex(a => a.Numero)
            .IsUnique();

        modelBuilder.Entity<Contratacao>()
            .Property(c => c.ValorSolicitado)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Contratacao>()
            .Property(c => c.TaxaJurosMensal)
            .HasPrecision(10, 4);

        modelBuilder.Entity<Contratacao>()
            .Property(c => c.ValorParcela)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PessoaFisica>()
            .Property(p => p.RendaMensal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PessoaJuridica>()
            .Property(p => p.FaturamentoMensal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Emprestimo>()
            .Property(e => e.ValorMinimo)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Emprestimo>()
            .Property(e => e.ValorMaximo)
            .HasPrecision(18, 2);

        modelBuilder.Entity<MaquinaDeCartao>()
            .Property(m => m.TaxaMdrPadrao)
            .HasPrecision(10, 4);

        modelBuilder.Entity<PessoaFisica>()
            .HasIndex(p => p.Cpf)
            .IsUnique();

        modelBuilder.Entity<PessoaJuridica>()
            .HasIndex(p => p.Cnpj)
            .IsUnique();

        modelBuilder.Entity<Cliente>()
            .HasOne(c => c.Agencia)
            .WithMany(a => a.Clientes)
            .HasForeignKey(c => c.AgenciaId);

        modelBuilder.Entity<Contratacao>()
            .HasOne(c => c.Cliente)
            .WithMany(c => c.Contratacoes)
            .HasForeignKey(c => c.ClienteId);

        modelBuilder.Entity<Contratacao>()
            .HasOne(c => c.Produto)
            .WithMany(p => p.Contratacoes)
            .HasForeignKey(c => c.ProdutoId);

        modelBuilder.Entity<Emprestimo>().HasData(new
        {
            Id = 1,
            Nome = "Emprestimo pessoal",
            ValorMinimo = 500m,
            ValorMaximo = 50000m,
            PrazoMaximoMeses = 48
        });

        foreach (var entidade in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var propriedade in entidade.GetProperties())
            {
                propriedade.SetColumnName(propriedade.GetColumnName().ToUpperInvariant());
            }

            foreach (var chave in entidade.GetKeys())
            {
                chave.SetName(chave.GetName()?.ToUpperInvariant());
            }

            foreach (var chaveEstrangeira in entidade.GetForeignKeys())
            {
                chaveEstrangeira.SetConstraintName(chaveEstrangeira.GetConstraintName()?.ToUpperInvariant());
            }

            foreach (var indice in entidade.GetIndexes())
            {
                indice.SetDatabaseName(indice.GetDatabaseName()?.ToUpperInvariant());
            }
        }
    }
}
