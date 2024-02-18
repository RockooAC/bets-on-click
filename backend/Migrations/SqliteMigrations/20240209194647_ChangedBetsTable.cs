using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations.SqliteMigrations
{
    public partial class ChangedBetsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BetEndTime",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "EventName",
                table: "Bets");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Bets",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Bets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bets_EventId",
                table: "Bets",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bets_Events_EventId",
                table: "Bets",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bets_Events_EventId",
                table: "Bets");

            migrationBuilder.DropIndex(
                name: "IX_Bets_EventId",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Bets");

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "Bets",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "BetEndTime",
                table: "Bets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventName",
                table: "Bets",
                type: "TEXT",
                nullable: true);
        }
    }
}
