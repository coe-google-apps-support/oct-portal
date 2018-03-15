using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdeaStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InitiativeId = table.Column<Guid>(nullable: false),
                    PersonId = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StatusEntryDateUtc = table.Column<DateTime>(nullable: false),
                    Text = table.Column<string>(maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdeaStatusHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Initiatives",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AlternateKey = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssigneeId = table.Column<int>(nullable: true),
                    BusinessCaseUrl = table.Column<string>(maxLength: 2048, nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    InvestmentRequestFormUrl = table.Column<string>(maxLength: 2048, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    WorkOrderId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Initiatives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StringTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 64, nullable: true),
                    Text = table.Column<string>(maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StringTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stakeholder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InitiativeId = table.Column<Guid>(nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stakeholder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stakeholder_Initiatives_InitiativeId",
                        column: x => x.InitiativeId,
                        principalTable: "Initiatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Initiatives_WorkOrderId",
                table: "Initiatives",
                column: "WorkOrderId",
                unique: true,
                filter: "[WorkOrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Stakeholder_InitiativeId",
                table: "Stakeholder",
                column: "InitiativeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdeaStatusHistories");

            migrationBuilder.DropTable(
                name: "Stakeholder");

            migrationBuilder.DropTable(
                name: "StringTemplates");

            migrationBuilder.DropTable(
                name: "Initiatives");
        }
    }
}
