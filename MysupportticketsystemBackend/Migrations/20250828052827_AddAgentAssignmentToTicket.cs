using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MysupportticketsystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentAssignmentToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedToAgentId",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignedToAgentId",
                table: "Tickets",
                column: "AssignedToAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_AssignedToAgentId",
                table: "Tickets",
                column: "AssignedToAgentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_AssignedToAgentId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_AssignedToAgentId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AssignedToAgentId",
                table: "Tickets");
        }
    }
}
