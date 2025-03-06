using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RealtorGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealtorGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupClients",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupClients", x => new { x.GroupId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_GroupClients_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupClients_RealtorGroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "RealtorGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Realtors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    RealtorGroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realtors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Realtors_RealtorGroup_RealtorGroupId",
                        column: x => x.RealtorGroupId,
                        principalTable: "RealtorGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Apartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Rooms = table.Column<int>(type: "integer", nullable: false),
                    Floor = table.Column<int>(type: "integer", nullable: false),
                    PhotoUrl = table.Column<string>(type: "text", nullable: true),
                    HouseView = table.Column<string>(type: "text", nullable: true),
                    Area = table.Column<double>(type: "double precision", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    RealtorId = table.Column<int>(type: "integer", nullable: false),
                    RealtorGroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apartments_RealtorGroup_RealtorGroupId",
                        column: x => x.RealtorGroupId,
                        principalTable: "RealtorGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Apartments_Realtors_RealtorId",
                        column: x => x.RealtorId,
                        principalTable: "Realtors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupRealtors",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    RealtorId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRealtors", x => new { x.GroupId, x.RealtorId });
                    table.ForeignKey(
                        name: "FK_GroupRealtors_RealtorGroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "RealtorGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupRealtors_Realtors_RealtorId",
                        column: x => x.RealtorId,
                        principalTable: "Realtors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    ApartmentId = table.Column<int>(type: "integer", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.InsertData(
                table: "ApartmentStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Available" },
                    { 2, "Sold" },
                    { 3, "Rented" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_RealtorGroupId",
                table: "Apartments",
                column: "RealtorGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_RealtorId",
                table: "Apartments",
                column: "RealtorId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Email",
                table: "Clients",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_ApartmentId",
                table: "Favorites",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupClients_ClientId",
                table: "GroupClients",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRealtors_RealtorId",
                table: "GroupRealtors",
                column: "RealtorId");

            migrationBuilder.CreateIndex(
                name: "IX_Realtors_Email",
                table: "Realtors",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realtors_RealtorGroupId",
                table: "Realtors",
                column: "RealtorGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApartmentStatuses");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "GroupClients");

            migrationBuilder.DropTable(
                name: "GroupRealtors");

            migrationBuilder.DropTable(
                name: "Apartments");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Realtors");

            migrationBuilder.DropTable(
                name: "RealtorGroup");
        }
    }
}
