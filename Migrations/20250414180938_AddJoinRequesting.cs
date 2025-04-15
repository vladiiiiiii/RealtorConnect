using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RealtorConnect.Migrations
{
    /// <inheritdoc />
    public partial class AddJoinRequesting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_GroupClients_GroupClientId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Realtors_GroupRealtors_GroupRealtorId",
                table: "Realtors");

            migrationBuilder.DropTable(
                name: "GroupClients");

            migrationBuilder.DropTable(
                name: "GroupRealtors");

            migrationBuilder.DropIndex(
                name: "IX_Realtors_GroupRealtorId",
                table: "Realtors");

            migrationBuilder.DropIndex(
                name: "IX_Clients_GroupClientId",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "GroupRealtorId",
                table: "Realtors",
                newName: "GroupId");

            migrationBuilder.CreateTable(
                name: "JoinRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealtorId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinRequests_RealtorGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "RealtorGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinRequests_Realtors_RealtorId",
                        column: x => x.RealtorId,
                        principalTable: "Realtors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Realtors_GroupId",
                table: "Realtors",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinRequests_GroupId",
                table: "JoinRequests",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinRequests_RealtorId",
                table: "JoinRequests",
                column: "RealtorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Realtors_RealtorGroups_GroupId",
                table: "Realtors",
                column: "GroupId",
                principalTable: "RealtorGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Realtors_RealtorGroups_GroupId",
                table: "Realtors");

            migrationBuilder.DropTable(
                name: "JoinRequests");

            migrationBuilder.DropIndex(
                name: "IX_Realtors_GroupId",
                table: "Realtors");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Realtors",
                newName: "GroupRealtorId");

            migrationBuilder.CreateTable(
                name: "GroupClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupClients_RealtorGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "RealtorGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupRealtors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    RealtorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRealtors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupRealtors_RealtorGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "RealtorGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Realtors_GroupRealtorId",
                table: "Realtors",
                column: "GroupRealtorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_GroupClientId",
                table: "Clients",
                column: "GroupClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupClients_GroupId",
                table: "GroupClients",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRealtors_GroupId",
                table: "GroupRealtors",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_GroupClients_GroupClientId",
                table: "Clients",
                column: "GroupClientId",
                principalTable: "GroupClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Realtors_GroupRealtors_GroupRealtorId",
                table: "Realtors",
                column: "GroupRealtorId",
                principalTable: "GroupRealtors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
