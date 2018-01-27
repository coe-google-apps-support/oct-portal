using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CoE.Ideas.ProjectManagement.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GitHub_Issue_Events",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Action = table.Column<string>(nullable: true),
                    AssigneeId = table.Column<long>(nullable: true),
                    AssignerId = table.Column<long>(nullable: true),
                    IssueId = table.Column<long>(nullable: true),
                    RepositoryId = table.Column<long>(nullable: true),
                    SenderId = table.Column<long>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHub_Issue_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GitHub_Repositories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AlternateKey = table.Column<string>(nullable: true),
                    ArchiveUrl = table.Column<string>(nullable: true),
                    AssigneesUrl = table.Column<string>(nullable: true),
                    BlobsUrl = table.Column<string>(nullable: true),
                    BranchesUrl = table.Column<string>(nullable: true),
                    CloneUrl = table.Column<string>(nullable: true),
                    CollaboratorsUrl = table.Column<string>(nullable: true),
                    CommentsUrl = table.Column<string>(nullable: true),
                    CommitsUrl = table.Column<string>(nullable: true),
                    CompareUrl = table.Column<string>(nullable: true),
                    ContentsUrl = table.Column<string>(nullable: true),
                    ContributorsUrl = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    DefaultBranch = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DownloadsUrl = table.Column<string>(nullable: true),
                    EventsUrl = table.Column<string>(nullable: true),
                    Forks = table.Column<int>(nullable: false),
                    ForksCount = table.Column<int>(nullable: false),
                    ForksUrl = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    GitCommitsUrl = table.Column<string>(nullable: true),
                    GitRefsUrl = table.Column<string>(nullable: true),
                    GitTagsUrl = table.Column<string>(nullable: true),
                    GitUrl = table.Column<string>(nullable: true),
                    HasDownloads = table.Column<bool>(nullable: false),
                    HasIssues = table.Column<bool>(nullable: false),
                    HasPages = table.Column<bool>(nullable: false),
                    HasWiki = table.Column<bool>(nullable: false),
                    HomePage = table.Column<string>(nullable: true),
                    HooksUrl = table.Column<string>(nullable: true),
                    HtmlUrl = table.Column<string>(nullable: true),
                    IsFork = table.Column<bool>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    IssueCommentUrl = table.Column<string>(nullable: true),
                    IssueEventsUrl = table.Column<string>(nullable: true),
                    IssuesUrl = table.Column<string>(nullable: true),
                    KeysUrl = table.Column<string>(nullable: true),
                    LabelsUrl = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: true),
                    LanguagesUrl = table.Column<string>(nullable: true),
                    MergesUrl = table.Column<string>(nullable: true),
                    MilstonesUrl = table.Column<string>(nullable: true),
                    MirrorUrl = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NotificationsUrl = table.Column<string>(nullable: true),
                    OpenIssues = table.Column<int>(nullable: false),
                    OpenIssuesCount = table.Column<int>(nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    PullsUrl = table.Column<string>(nullable: true),
                    PushedAt = table.Column<DateTimeOffset>(nullable: false),
                    ReleasesUrl = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: false),
                    SshUrl = table.Column<string>(nullable: true),
                    StargazersCount = table.Column<int>(nullable: false),
                    StargazersUrl = table.Column<string>(nullable: true),
                    StatusesUrl = table.Column<string>(nullable: true),
                    SubscribersUrl = table.Column<string>(nullable: true),
                    SubscriptionUrl = table.Column<string>(nullable: true),
                    SvcUrl = table.Column<string>(nullable: true),
                    TagsUrl = table.Column<string>(nullable: true),
                    TeamsUrl = table.Column<string>(nullable: true),
                    TreesUrl = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Watchers = table.Column<int>(nullable: false),
                    WatchersCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHub_Repositories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Issues",
                columns: table => new
                {
                    AssigneeId = table.Column<long>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    ClosedAt = table.Column<DateTimeOffset>(nullable: true),
                    Comments = table.Column<string>(nullable: true),
                    CommentsUrl = table.Column<string>(nullable: true),
                    EventsUrl = table.Column<string>(nullable: true),
                    HtmlUrl = table.Column<string>(nullable: true),
                    IsLocked = table.Column<bool>(nullable: true),
                    LabelsUrl = table.Column<string>(nullable: true),
                    Milestone = table.Column<string>(nullable: true),
                    Number = table.Column<int>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AlternateKey = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    IssueStatus = table.Column<int>(nullable: false),
                    ProjectManagementSystem = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GitHub_Labels",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AlternateKey = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    GitHubIssueInternalId = table.Column<long>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHub_Labels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GitHub_Labels_Issues_GitHubIssueInternalId",
                        column: x => x.GitHubIssueInternalId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GitHub_Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AlternateKey = table.Column<string>(nullable: true),
                    AvatarUrl = table.Column<string>(nullable: true),
                    EventsUrl = table.Column<string>(nullable: true),
                    FollowersUrl = table.Column<string>(nullable: true),
                    FollowingUrl = table.Column<string>(nullable: true),
                    GistsUrl = table.Column<string>(nullable: true),
                    GitHubIssueInternalId = table.Column<long>(nullable: true),
                    GravatarId = table.Column<string>(nullable: true),
                    HtmlUrl = table.Column<string>(nullable: true),
                    IsSiteAdmin = table.Column<bool>(nullable: false),
                    Login = table.Column<string>(nullable: true),
                    OrganizationsUrl = table.Column<string>(nullable: true),
                    ReceivedEventsUrl = table.Column<string>(nullable: true),
                    ReposUrl = table.Column<string>(nullable: true),
                    StarredUrl = table.Column<string>(nullable: true),
                    SubscriptionsUrl = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHub_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GitHub_Users_Issues_GitHubIssueInternalId",
                        column: x => x.GitHubIssueInternalId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Issue_Status_Changes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChangeDate = table.Column<DateTimeOffset>(nullable: false),
                    IssueId = table.Column<long>(nullable: true),
                    NewStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issue_Status_Changes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Issue_Status_Changes_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GitHub_Issue_Events_AssigneeId",
                table: "GitHub_Issue_Events",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_GitHub_Issue_Events_AssignerId",
                table: "GitHub_Issue_Events",
                column: "AssignerId");

            migrationBuilder.CreateIndex(
                name: "IX_GitHub_Issue_Events_IssueId",
                table: "GitHub_Issue_Events",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_GitHub_Issue_Events_RepositoryId",
                table: "GitHub_Issue_Events",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GitHub_Issue_Events_SenderId",
                table: "GitHub_Issue_Events",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_GitHub_Labels_GitHubIssueInternalId",
                table: "GitHub_Labels",
                column: "GitHubIssueInternalId");

            migrationBuilder.CreateIndex(
                name: "IX_GitHub_Repositories_OwnerId",
                table: "GitHub_Repositories",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_GitHub_Users_GitHubIssueInternalId",
                table: "GitHub_Users",
                column: "GitHubIssueInternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Issue_Status_Changes_IssueId",
                table: "Issue_Status_Changes",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_AssigneeId",
                table: "Issues",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_UserId",
                table: "Issues",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GitHub_Issue_Events_GitHub_Users_AssigneeId",
                table: "GitHub_Issue_Events",
                column: "AssigneeId",
                principalTable: "GitHub_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GitHub_Issue_Events_GitHub_Users_AssignerId",
                table: "GitHub_Issue_Events",
                column: "AssignerId",
                principalTable: "GitHub_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GitHub_Issue_Events_GitHub_Users_SenderId",
                table: "GitHub_Issue_Events",
                column: "SenderId",
                principalTable: "GitHub_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GitHub_Issue_Events_Issues_IssueId",
                table: "GitHub_Issue_Events",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GitHub_Issue_Events_GitHub_Repositories_RepositoryId",
                table: "GitHub_Issue_Events",
                column: "RepositoryId",
                principalTable: "GitHub_Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GitHub_Repositories_GitHub_Users_OwnerId",
                table: "GitHub_Repositories",
                column: "OwnerId",
                principalTable: "GitHub_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_GitHub_Users_AssigneeId",
                table: "Issues",
                column: "AssigneeId",
                principalTable: "GitHub_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_GitHub_Users_UserId",
                table: "Issues",
                column: "UserId",
                principalTable: "GitHub_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_GitHub_Users_AssigneeId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_GitHub_Users_UserId",
                table: "Issues");

            migrationBuilder.DropTable(
                name: "GitHub_Issue_Events");

            migrationBuilder.DropTable(
                name: "GitHub_Labels");

            migrationBuilder.DropTable(
                name: "Issue_Status_Changes");

            migrationBuilder.DropTable(
                name: "GitHub_Repositories");

            migrationBuilder.DropTable(
                name: "GitHub_Users");

            migrationBuilder.DropTable(
                name: "Issues");
        }
    }
}
