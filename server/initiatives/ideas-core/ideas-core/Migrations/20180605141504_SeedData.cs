using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CoE.Ideas.Core.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO StatusEtas (EtaType,Status,Time) VALUES(" +
    (int)Data.EtaType.BusinessSeconds + "," +
    (int)Data.InitiativeStatus.Submit + ",14400);");

            migrationBuilder.Sql(@"
INSERT INTO StringTemplates(`Category`, `Key`, `Text`) VALUES(1, 'Submit_present', 'Thank you! Your request has been submitted and will be assigned for review. An OCT representative will contact you by {1}');
INSERT INTO StringTemplates(`Category`, `Key`, `Text`) VALUES(1, 'Submit_past', 'Thank you! Your initiative was submitted.');
INSERT INTO StringTemplates(`Category`, `Key`, `Text`) VALUES(1, 'Review_present', 'Your request has been assigned for review. {0} will contact you if more information is required in order to properly route request.');
INSERT INTO StringTemplates(`Category`, `Key`, `Text`) VALUES(1, 'Review_past', 'Your initiative has been assigned and reviewed.');
INSERT INTO StringTemplates(`Category`, `Key`, `Text`) VALUES(1, 'Collaborate_present', '{0} is actively working to complete your request. with you to complete an Investment Request for your initiative so that it may be prioritized and approved.');
INSERT INTO StringTemplates(`Category`, `Key`, `Text`) VALUES(1, 'Collaborate_past', 'An Investment Request has been completed for your initiative.');
INSERT INTO StringTemplates(`Category`, `Key`, `Text`) VALUES(1, 'Deliver_present', 'Your request has been submitted to Technology Investment for Business Technology Steering Committee Approval. A Technology Investment representative will notify you when your initiative has been approved.');
INSERT INTO StringTemplates(`Category`, `Key`, `Text`) VALUES(1, 'Deliver_past', 'Your initiative has been successfully submitted as a project with Solutions Delivery.');
INSERT INTO StringTemplates(`Category`, `Key`, `Text`) VALUES(1, 'Cancelled_present', 'Your request has been cancelled. ');
INSERT INTO StringTemplates(`Category`, `Key`, `Text`) VALUES(1, 'Cancelled_past', 'Your request was cancelled');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM StringTemplates WHERE Category = 1 AND [Key] IN ('Cancelled_past', 'Cancelled_present', 'Submit_past', 'Submit_present', 'Review_past', 'Review_present', 'Collaborate_past', 'Collaborate_present', 'Deliver_past', 'Deliver_present');");

            migrationBuilder.Sql(@"
DELETE FROM [dbo].[StatusEtas] WHERE [EtaType] = " + (int)Data.EtaType.BusinessSeconds + " AND [Status] = " + (int)Data.InitiativeStatus.Submit);

        }
    }
}
