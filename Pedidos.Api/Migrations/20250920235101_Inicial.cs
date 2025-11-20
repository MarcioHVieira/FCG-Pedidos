using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pedidos.Api.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pedidos");

            migrationBuilder.CreateTable(
                name: "Pedidos",
                schema: "pedidos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioNome = table.Column<string>(type: "varchar(100)", nullable: false),
                    JogoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JogoTitulo = table.Column<string>(type: "varchar(100)", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pedidos",
                schema: "pedidos");
        }
    }
}
