using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.Core.Migrations
{
    public partial class MoreSecondaryFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BusinessSponsor",
                table: "Ideas",
                newName: "OneCityAlignment");

            migrationBuilder.AddColumn<string>(
                name: "BusinessBenefites",
                table: "Ideas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessSponsorEmail",
                table: "Ideas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessSponsorName",
                table: "Ideas",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "Ideas",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: true, defaultValue: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BranchId = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: true, defaultValue: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_DepartmentId",
                table: "Ideas",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_BranchId",
                table: "Departments",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_Departments_DepartmentId",
                table: "Ideas",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_Departments_DepartmentId",
                table: "Ideas");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Ideas_DepartmentId",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "BusinessBenefites",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "BusinessSponsorEmail",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "BusinessSponsorName",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Ideas");

            migrationBuilder.RenameColumn(
                name: "OneCityAlignment",
                table: "Ideas",
                newName: "BusinessSponsor");
        }
    }
}
