using Microsoft.EntityFrameworkCore;
using ProjetoBanco.Api.Dados;
using ProjetoBanco.Api.Filas;
using ProjetoBanco.Api.Servicos;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opcoes =>
    {
        opcoes.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opcoes.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsEnvironment("Testes"))
{
    builder.Services.AddDbContext<BancoContexto>(opcoes =>
        opcoes.UseInMemoryDatabase("ProjetoBancoTestes"));
    builder.Services.AddSingleton<IFilaContratacao, FilaContratacaoMemoria>();
}
else
{
    builder.Services.AddDbContext<BancoContexto>(opcoes =>
        opcoes.UseOracle(
            builder.Configuration.GetConnectionString("Oracle"),
            oracle => oracle.MigrationsHistoryTable("PB_MIGRACOES_EF")));
    builder.Services.AddSingleton<IFilaContratacao, FilaContratacaoRabbitMq>();
}

builder.Services.AddScoped<ServicoScore>();
builder.Services.AddScoped<ServicoEmprestimo>();
builder.Services.AddHostedService<ProcessadorContratacao>();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testes"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testes"))
{
    app.UseHttpsRedirection();
}
app.MapControllers();

app.Run();

public partial class Program
{
}
