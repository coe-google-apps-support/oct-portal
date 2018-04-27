using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.Core.Migrations
{
    public partial class MakeApexIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ApexId",
                table: "Initiatives",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ApexId",
                table: "Initiatives",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
