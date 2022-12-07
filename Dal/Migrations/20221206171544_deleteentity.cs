using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dal.Migrations
{
    /// <inheritdoc />
    public partial class deleteentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_MoviePremiereUpdateLogs_MoviePremiereUpdateLogId",
                table: "Movies");

            migrationBuilder.DropTable(
                name: "MoviePremiereUpdateLogs");

            migrationBuilder.DropIndex(
                name: "IX_Movies_MoviePremiereUpdateLogId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "MoviePremiereUpdateLogId",
                table: "Movies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MoviePremiereUpdateLogId",
                table: "Movies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MoviePremiereUpdateLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviePremiereUpdateLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_MoviePremiereUpdateLogId",
                table: "Movies",
                column: "MoviePremiereUpdateLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_MoviePremiereUpdateLogs_MoviePremiereUpdateLogId",
                table: "Movies",
                column: "MoviePremiereUpdateLogId",
                principalTable: "MoviePremiereUpdateLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
