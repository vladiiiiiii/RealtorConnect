using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RealtorConnect.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApartmentStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RealtorGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealtorGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false)
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
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupClientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_GroupClients_GroupClientId",
                        column: x => x.GroupClientId,
                        principalTable: "GroupClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Apartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rooms = table.Column<int>(type: "int", nullable: false),
                    Floor = table.Column<int>(type: "int", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseView = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SquareArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    RealtorId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    ApartmentStatusId = table.Column<int>(type: "int", nullable: false),
                    RealtorGroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apartments_ApartmentStatuses_ApartmentStatusId",
                        column: x => x.ApartmentStatusId,
                        principalTable: "ApartmentStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Apartments_ApartmentStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ApartmentStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Apartments_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Apartments_RealtorGroups_RealtorGroupId",
                        column: x => x.RealtorGroupId,
                        principalTable: "RealtorGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ApartmentId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => new { x.ClientId, x.ApartmentId });
                    table.ForeignKey(
                        name: "FK_Favorites_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ViewHistories",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ApartmentId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewHistories", x => new { x.ClientId, x.ApartmentId });
                    table.ForeignKey(
                        name: "FK_ViewHistories_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViewHistories_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderClientId = table.Column<int>(type: "int", nullable: true),
                    SenderRealtorId = table.Column<int>(type: "int", nullable: true),
                    ReceiverClientId = table.Column<int>(type: "int", nullable: true),
                    ReceiverRealtorId = table.Column<int>(type: "int", nullable: true),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Clients_ReceiverClientId",
                        column: x => x.ReceiverClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Clients_SenderClientId",
                        column: x => x.SenderClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupRealtors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    RealtorId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Realtors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupRealtorId = table.Column<int>(type: "int", nullable: true),
                    RealtorGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realtors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Realtors_GroupRealtors_GroupRealtorId",
                        column: x => x.GroupRealtorId,
                        principalTable: "GroupRealtors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Realtors_RealtorGroups_RealtorGroupId",
                        column: x => x.RealtorGroupId,
                        principalTable: "RealtorGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ApartmentStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Доступна" },
                    { 2, "Недоступна" }
                });

            migrationBuilder.InsertData(
                table: "RealtorGroups",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Default Group" });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_ApartmentStatusId",
                table: "Apartments",
                column: "ApartmentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_ClientId",
                table: "Apartments",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_RealtorGroupId",
                table: "Apartments",
                column: "RealtorGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_RealtorId",
                table: "Apartments",
                column: "RealtorId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_StatusId",
                table: "Apartments",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ReceiverClientId",
                table: "ChatMessages",
                column: "ReceiverClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ReceiverRealtorId",
                table: "ChatMessages",
                column: "ReceiverRealtorId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderClientId",
                table: "ChatMessages",
                column: "SenderClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderRealtorId",
                table: "ChatMessages",
                column: "SenderRealtorId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_GroupClientId",
                table: "Clients",
                column: "GroupClientId",
                unique: true,
                filter: "[GroupClientId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_ApartmentId",
                table: "Favorites",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupClients_GroupId",
                table: "GroupClients",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRealtors_GroupId",
                table: "GroupRealtors",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRealtors_RealtorId",
                table: "GroupRealtors",
                column: "RealtorId");

            migrationBuilder.CreateIndex(
                name: "IX_Realtors_GroupRealtorId",
                table: "Realtors",
                column: "GroupRealtorId",
                unique: true,
                filter: "[GroupRealtorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Realtors_RealtorGroupId",
                table: "Realtors",
                column: "RealtorGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewHistories_ApartmentId",
                table: "ViewHistories",
                column: "ApartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Realtors_RealtorId",
                table: "Apartments",
                column: "RealtorId",
                principalTable: "Realtors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Realtors_ReceiverRealtorId",
                table: "ChatMessages",
                column: "ReceiverRealtorId",
                principalTable: "Realtors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Realtors_SenderRealtorId",
                table: "ChatMessages",
                column: "SenderRealtorId",
                principalTable: "Realtors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupRealtors_Realtors_RealtorId",
                table: "GroupRealtors",
                column: "RealtorId",
                principalTable: "Realtors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupRealtors_RealtorGroups_GroupId",
                table: "GroupRealtors");

            migrationBuilder.DropForeignKey(
                name: "FK_Realtors_RealtorGroups_RealtorGroupId",
                table: "Realtors");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupRealtors_Realtors_RealtorId",
                table: "GroupRealtors");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "ViewHistories");

            migrationBuilder.DropTable(
                name: "Apartments");

            migrationBuilder.DropTable(
                name: "ApartmentStatuses");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "GroupClients");

            migrationBuilder.DropTable(
                name: "RealtorGroups");

            migrationBuilder.DropTable(
                name: "Realtors");

            migrationBuilder.DropTable(
                name: "GroupRealtors");
        }
    }
}
