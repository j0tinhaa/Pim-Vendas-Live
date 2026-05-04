using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PimVendas.Migrations
{
    /// <inheritdoc />
    public partial class RemoverProdutoUnificarVenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Pré-check: aborta com mensagem clara se já existem vendas duplicadas (LiveId, CodigoProduto).
            // Esses códigos hoje passam (índice unique era global na tabela Produtos, não na Venda),
            // mas se duas vendas referenciam o mesmo Produto na mesma Live, a constraint nova quebra.
            // Vendedora deve renomear ou cancelar manualmente antes de rodar a migration de novo.
            migrationBuilder.Sql(@"
DECLARE @duplicados INT;
SELECT @duplicados = COUNT(*) FROM (
    SELECT LiveId, CodigoProduto
    FROM Vendas
    GROUP BY LiveId, CodigoProduto
    HAVING COUNT(*) > 1
) AS d;

IF @duplicados > 0
BEGIN
    DECLARE @msg NVARCHAR(MAX) = N'Migration abortada: existem ' + CAST(@duplicados AS NVARCHAR(10)) +
        N' combinações (LiveId, CodigoProduto) duplicadas em Vendas. ' +
        N'Liste com: SELECT LiveId, CodigoProduto, COUNT(*) qtd FROM Vendas GROUP BY LiveId, CodigoProduto HAVING COUNT(*) > 1. ' +
        N'Renomeie ou cancele as vendas conflitantes antes de rodar a migration novamente.';
    THROW 50000, @msg, 1;
END;
");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId",
                table: "Vendas");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_LiveId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_ProdutoId",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "ProdutoId",
                table: "Vendas");

            migrationBuilder.RenameColumn(
                name: "NomeProduto",
                table: "Vendas",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "CodigoProduto",
                table: "Vendas",
                newName: "Codigo");

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Vendas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_LiveId_Codigo",
                table: "Vendas",
                columns: new[] { "LiveId", "Codigo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vendas_LiveId_Codigo",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Vendas");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Vendas",
                newName: "NomeProduto");

            migrationBuilder.RenameColumn(
                name: "Codigo",
                table: "Vendas",
                newName: "CodigoProduto");

            migrationBuilder.AddColumn<int>(
                name: "ProdutoId",
                table: "Vendas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Cor = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Tamanho = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_LiveId",
                table: "Vendas",
                column: "LiveId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ProdutoId",
                table: "Vendas",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Codigo",
                table: "Produtos",
                column: "Codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId",
                table: "Vendas",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
