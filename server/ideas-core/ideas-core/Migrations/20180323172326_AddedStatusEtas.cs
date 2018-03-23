using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.Core.Migrations
{
    public partial class AddedStatusEtas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatusEtas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EtaType = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Time = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusEtas", x => x.Id);
                });
            AddSeedData(migrationBuilder);
        }

        private void AddSeedData(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [dbo].[StatusEtas] ([EtaType],[Status],[Time]) VALUES(1,3,14400)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusEtas");
        }

    }
}
