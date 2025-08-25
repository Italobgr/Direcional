using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Direcional.Api.Migrations
{
    /// <inheritdoc />
    public partial class Add_Unique_ReservaAtiva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reserva_Apto_Status",
                table: "Reservas");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "UX_Reservas_Apto_Ativa",
                table: "Reservas",
                column: "IdApartamento",
                unique: true,
                filter: "[Status] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Reservas_Apto_Ativa",
                table: "Reservas");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Reservas",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_Apto_Status",
                table: "Reservas",
                columns: new[] { "IdApartamento", "Status" });
        }
    }
}
