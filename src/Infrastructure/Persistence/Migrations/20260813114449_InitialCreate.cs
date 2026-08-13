using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    NameTh = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DocumentName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    StatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_DocumentStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "DocumentStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DocumentId = table.Column<int>(type: "INTEGER", nullable: false),
                    FromStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    ToStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ActionBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ActionAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalLog_DocumentStatus_FromStatusId",
                        column: x => x.FromStatusId,
                        principalTable: "DocumentStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalLog_DocumentStatus_ToStatusId",
                        column: x => x.ToStatusId,
                        principalTable: "DocumentStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalLog_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DocumentStatus",
                columns: new[] { "Id", "Code", "NameTh" },
                values: new object[,]
                {
                    { 1, "PENDING", "รออนุมัติ" },
                    { 2, "APPROVED", "อนุมัติ" },
                    { 3, "REJECTED", "ไม่อนุมัติ" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLog_DocumentId",
                table: "ApprovalLog",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLog_FromStatusId",
                table: "ApprovalLog",
                column: "FromStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLog_ToStatusId",
                table: "ApprovalLog",
                column: "ToStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_StatusId",
                table: "Documents",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentStatus_Code",
                table: "DocumentStatus",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalLog");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "DocumentStatus");
        }
    }
}
