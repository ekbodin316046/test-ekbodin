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
                name: "document_status",
                columns: table => new
                {
                    status_id = table.Column<int>(type: "INTEGER", nullable: false),
                    status_code = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    status_name_tha = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    created_by = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    created_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_program = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    updated_by = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    updated_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_program = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_status", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    document_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    document_name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    status_id = table.Column<int>(type: "INTEGER", nullable: false),
                    created_by = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    created_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_program = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    updated_by = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    updated_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_program = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documents", x => x.document_id);
                    table.ForeignKey(
                        name: "FK_documents_document_status_status_id",
                        column: x => x.status_id,
                        principalTable: "document_status",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "approval_log",
                columns: table => new
                {
                    approval_log_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    document_id = table.Column<int>(type: "INTEGER", nullable: false),
                    from_status_id = table.Column<int>(type: "INTEGER", nullable: false),
                    to_status_id = table.Column<int>(type: "INTEGER", nullable: false),
                    reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    created_by = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    created_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    created_program = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    updated_by = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    updated_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_program = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval_log", x => x.approval_log_id);
                    table.ForeignKey(
                        name: "FK_approval_log_document_status_from_status_id",
                        column: x => x.from_status_id,
                        principalTable: "document_status",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_approval_log_document_status_to_status_id",
                        column: x => x.to_status_id,
                        principalTable: "document_status",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_approval_log_documents_document_id",
                        column: x => x.document_id,
                        principalTable: "documents",
                        principalColumn: "document_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "document_status",
                columns: new[] { "status_id", "status_code", "created_by", "created_date", "created_program", "status_name_tha", "updated_by", "updated_date", "updated_program" },
                values: new object[,]
                {
                    { 1, "PENDING", "SYSTEM", new DateTime(2026, 8, 1, 9, 0, 0, 0, DateTimeKind.Utc), "SEED", "รออนุมัติ", "SYSTEM", new DateTime(2026, 8, 1, 9, 0, 0, 0, DateTimeKind.Utc), "SEED" },
                    { 2, "APPROVED", "SYSTEM", new DateTime(2026, 8, 1, 9, 0, 0, 0, DateTimeKind.Utc), "SEED", "อนุมัติ", "SYSTEM", new DateTime(2026, 8, 1, 9, 0, 0, 0, DateTimeKind.Utc), "SEED" },
                    { 3, "REJECTED", "SYSTEM", new DateTime(2026, 8, 1, 9, 0, 0, 0, DateTimeKind.Utc), "SEED", "ไม่อนุมัติ", "SYSTEM", new DateTime(2026, 8, 1, 9, 0, 0, 0, DateTimeKind.Utc), "SEED" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_approval_log_document_id",
                table: "approval_log",
                column: "document_id");

            migrationBuilder.CreateIndex(
                name: "IX_approval_log_from_status_id",
                table: "approval_log",
                column: "from_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_approval_log_to_status_id",
                table: "approval_log",
                column: "to_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_document_status_status_code",
                table: "document_status",
                column: "status_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_documents_status_id",
                table: "documents",
                column: "status_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "approval_log");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "document_status");
        }
    }
}
