using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveStore.Migrations
{
    /// <inheritdoc />
    public partial class NovoBanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cliente",
                table: "Vendas");

            migrationBuilder.RenameColumn(
                name: "Produto",
                table: "Vendas",
                newName: "NomeProduto");

            migrationBuilder.AddColumn<string>(
                name: "ClienteInstagram",
                table: "Vendas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LiveId",
                table: "Vendas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProdutoId",
                table: "Vendas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    InstagramUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.InstagramUser);
                });

            migrationBuilder.CreateTable(
                name: "Lives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IniciadaEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EncerradaEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Cor = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Tamanho = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ClienteInstagram",
                table: "Vendas",
                column: "ClienteInstagram");

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
                name: "FK_Vendas_Clientes_ClienteInstagram",
                table: "Vendas",
                column: "ClienteInstagram",
                principalTable: "Clientes",
                principalColumn: "InstagramUser",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Lives_LiveId",
                table: "Vendas",
                column: "LiveId",
                principalTable: "Lives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId",
                table: "Vendas",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Clientes_ClienteInstagram",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Lives_LiveId",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId",
                table: "Vendas");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Lives");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_ClienteInstagram",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_LiveId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_ProdutoId",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "ClienteInstagram",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "LiveId",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "ProdutoId",
                table: "Vendas");

            migrationBuilder.RenameColumn(
                name: "NomeProduto",
                table: "Vendas",
                newName: "Produto");

            migrationBuilder.AddColumn<string>(
                name: "Cliente",
                table: "Vendas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
