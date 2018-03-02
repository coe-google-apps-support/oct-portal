using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.Core.Migrations
{
    public partial class AddingIdeaStatusHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "StringTemplates",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IdeaStatusHistories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InitiativeId = table.Column<long>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StatusEntryDateUtc = table.Column<DateTime>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdeaStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdeaStatusHistories_Ideas_InitiativeId",
                        column: x => x.InitiativeId,
                        principalTable: "Ideas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdeaStatusHistories_InitiativeId",
                table: "IdeaStatusHistories",
                column: "InitiativeId");

            migrationBuilder.Sql(@"
INSERT INTO StringTemplates(Category, `key`, `Text`) VALUES(1, 'Submit_present', 'Thank you! Your initiative has been submitted and will be assigned for review. An OCT representative will contact you within three (3) business days.');
INSERT INTO StringTemplates(Category, `key`, `Text`) VALUES(1, 'Submit_past', 'Thank you! Your initiative was submitted.');
INSERT INTO StringTemplates(Category, `key`, `Text`) VALUES(1, 'Review_present', 'Your initiative has been assigned for review. {0} will contact you to schedule a project intake meeting.');
INSERT INTO StringTemplates(Category, `key`, `Text`) VALUES(1, 'Review_past', 'Your initiative has been assigned and reviewed.');
INSERT INTO StringTemplates(Category, `key`, `Text`) VALUES(1, 'Collaborate_present', '{0} is actively working with you to complete an Investment Request for your initiative.');
INSERT INTO StringTemplates(Category, `key`, `Text`) VALUES(1, 'Collaborate_past', 'An Investment Request has been completed for your initiative.');
INSERT INTO StringTemplates(Category, `key`, `Text`) VALUES(1, 'Deliver_present', 'Your request has been successfully submitted as a project with Solutions Delivery. A representative from Solutions Delivery will contact you to discuss the next steps.');
INSERT INTO StringTemplates(Category, `key`, `Text`) VALUES(1, 'Deliver_past', 'Your initiative has been successfully submitted as a project with Solutions Delivery.');
INSERT INTO StringTemplates(Category, `key`, `Text`) VALUES(1, 'Cancelled_preset', 'Your request has been cancelled. ');
INSERT INTO StringTemplates(Category, `key`, `Text`) VALUES(1, 'Cancelled_past', 'Your request was cancelled');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM StringTemplates WHERE Category = 1 AND `Key` IN ('Cancelled_past', 'Cancelled_present', 'Submit_past', 'Submit_present', 'Review_past', 'Review_present', 'Collaborate_past', 'Collaborate_present', 'Deliver_past', 'Deliver_present');");

            migrationBuilder.DropTable(
                name: "IdeaStatusHistories");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "StringTemplates");
        }
    }
}
