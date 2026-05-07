using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjetoBanco.Api.Dados;
using ProjetoBanco.Api.Filas;

namespace ProjetoBanco.Testes;

public class FabricaApi : WebApplicationFactory<Program>
{
    private readonly string _nomeBanco = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testes");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<BancoContexto>>();
            services.RemoveAll<IFilaContratacao>();

            services.AddDbContext<BancoContexto>(opcoes =>
                opcoes.UseInMemoryDatabase(_nomeBanco));
            services.AddSingleton<IFilaContratacao, FilaContratacaoMemoria>();

            using var escopo = services.BuildServiceProvider().CreateScope();
            var contexto = escopo.ServiceProvider.GetRequiredService<BancoContexto>();
            contexto.Database.EnsureDeleted();
            contexto.Database.EnsureCreated();
        });
    }
}
