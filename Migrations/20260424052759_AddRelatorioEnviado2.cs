using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PimVendas.Migrations
{
    /// <inheritdoc />
    public partial class AddRelatorioEnviado2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelatoriosEnviados",
                columns: table => new
                {
                    LiveId = table.Column<int>(type: "int", nullable: false),
                    ClienteInstagram = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Enviado = table.Column<bool>(type: "bit", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatoriosEnviados", x => new { x.LiveId, x.ClienteInstagram });
                    table.ForeignKey(
                        name: "FK_RelatoriosEnviados_Clientes_ClienteInstagram",
                        column: x => x.ClienteInstagram,
                        principalTable: "Clientes",
                        principalColumn: "InstagramUser",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelatoriosEnviados_Lives_LiveId",
                        column: x => x.LiveId,
                        principalTable: "Lives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelatoriosEnviados_ClienteInstagram",
                table: "RelatoriosEnviados",
                column: "ClienteInstagram");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelatoriosEnviados");
        }
    }
}
