using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoBanco.Api.Dados.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PB_AGENCIAS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NUMERO = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ENDERECO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PB_AGENCIAS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PB_PRODUTOS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TIPOPRODUTO = table.Column<string>(type: "NVARCHAR2(21)", maxLength: 21, nullable: false),
                    VALORMINIMO = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    VALORMAXIMO = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    PRAZOMAXIMOMESES = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    TAXAMDRPADRAO = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: true),
                    CONVENIOEMPRESA = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PB_PRODUTOS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PB_CLIENTES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    AGENCIAID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TIPOCLIENTE = table.Column<string>(type: "NVARCHAR2(21)", maxLength: 21, nullable: false),
                    CPF = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    DATANASCIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    RENDAMENSAL = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    CNPJ = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    RAZAOSOCIAL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FATURAMENTOMENSAL = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PB_CLIENTES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PB_CLIENTES_PB_AGENCIAS_AGENCIAID",
                        column: x => x.AGENCIAID,
                        principalTable: "PB_AGENCIAS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PB_CONTRATACOES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CLIENTEID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PRODUTOID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DATASOLICITACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    STATUS = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VALORSOLICITADO = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    PRAZOMESES = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SCORECALCULADO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TAXAJUROSMENSAL = table.Column<decimal>(type: "DECIMAL(10,4)", precision: 10, scale: 4, nullable: false),
                    VALORPARCELA = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    MENSAGEM = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PB_CONTRATACOES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PB_CONTRATACOES_PB_CLIENTES_CLIENTEID",
                        column: x => x.CLIENTEID,
                        principalTable: "PB_CLIENTES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PB_CONTRATACOES_PB_PRODUTOS_PRODUTOID",
                        column: x => x.PRODUTOID,
                        principalTable: "PB_PRODUTOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PB_PRODUTOS",
                columns: new[] { "ID", "NOME", "PRAZOMAXIMOMESES", "TIPOPRODUTO", "VALORMAXIMO", "VALORMINIMO" },
                values: new object[] { 1, "Emprestimo pessoal", 48, "Emprestimo", 50000m, 500m });

            migrationBuilder.CreateIndex(
                name: "IX_PB_AGENCIAS_NUMERO",
                table: "PB_AGENCIAS",
                column: "NUMERO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PB_CLIENTES_AGENCIAID",
                table: "PB_CLIENTES",
                column: "AGENCIAID");

            migrationBuilder.CreateIndex(
                name: "IX_PB_CLIENTES_CNPJ",
                table: "PB_CLIENTES",
                column: "CNPJ",
                unique: true,
                filter: "\"CNPJ\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PB_CLIENTES_CPF",
                table: "PB_CLIENTES",
                column: "CPF",
                unique: true,
                filter: "\"CPF\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PB_CONTRATACOES_CLIENTEID",
                table: "PB_CONTRATACOES",
                column: "CLIENTEID");

            migrationBuilder.CreateIndex(
                name: "IX_PB_CONTRATACOES_PRODUTOID",
                table: "PB_CONTRATACOES",
                column: "PRODUTOID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PB_CONTRATACOES");

            migrationBuilder.DropTable(
                name: "PB_CLIENTES");

            migrationBuilder.DropTable(
                name: "PB_PRODUTOS");

            migrationBuilder.DropTable(
                name: "PB_AGENCIAS");
        }
    }
}
