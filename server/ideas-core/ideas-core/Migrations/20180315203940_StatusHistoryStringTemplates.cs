using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.Core.Migrations
{
    public partial class StatusHistoryStringTemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Submit_present', 'Thank you! Your initiative has been submitted and will be assigned for review. An OCT representative will contact you by {0}')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Submit_past', 'Thank you! Your initiative was submitted.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Review_present', 'Your initiative has been assigned for review. {0} will be working with you to further define your initiative.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Review_past', 'Your initiative has been assigned and reviewed.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Collaborate_present', '{0} is actively working with you to complete an Investment Request for your initiative so that it may be prioritized and approved.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Collaborate_past', 'An Investment Request has been completed for your initiative.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Deliver_present', 'Your request has been submitted to Technology Investment for Business Technology Steering Committee Approval. A Technology Investment representative will notify you when your initiative has been approved.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Deliver_past', 'Your initiative has been successfully submitted as a project with Solutions Delivery.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Cancelled_present', 'Your request has been cancelled. ')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Cancelled_past', 'Your request was cancelled')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM StringTemplates WHERE Category = 1 AND [Key] IN ('Cancelled_past', 'Cancelled_present', 'Submit_past', 'Submit_present', 'Review_past', 'Review_present', 'Collaborate_past', 'Collaborate_present', 'Deliver_past', 'Deliver_present');");

        }
    }
}
