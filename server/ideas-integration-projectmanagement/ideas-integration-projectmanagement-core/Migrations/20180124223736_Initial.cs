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
            migrationBuilder.EnsureSchema(
                name: "GitHub");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "GitHub",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlternateKey = table.Column<string>(nullable: true),
                    AvatarUrl = table.Column<string>(nullable: true),
                    EventsUrl = table.Column<string>(nullable: true),
                    FollowersUrl = table.Column<string>(nullable: true),
                    FollowingUrl = table.Column<string>(nullable: true),
                    GistsUrl = table.Column<string>(nullable: true),
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
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Issues",
                columns: table => new
                {
                    Assignee = table.Column<string>(nullable: true),
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
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                    table.ForeignKey(
                        name: "FK_Issues_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "GitHub",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Repositories",
                schema: "GitHub",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                    table.PrimaryKey("PK_Repositories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repositories_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "GitHub",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssueStatusChanges",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChangeDate = table.Column<DateTimeOffset>(nullable: false),
                    IssueId = table.Column<long>(nullable: true),
                    NewStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueStatusChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueStatusChanges_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                schema: "GitHub",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlternateKey = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    GitHubIssueInternalId = table.Column<long>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Labels_Issues_GitHubIssueInternalId",
                        column: x => x.GitHubIssueInternalId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssueEvents",
                schema: "GitHub",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Action = table.Column<string>(nullable: true),
                    IssueId = table.Column<long>(nullable: true),
                    RepositoryId = table.Column<long>(nullable: true),
                    SenderId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueEvents_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssueEvents_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalSchema: "GitHub",
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssueEvents_Users_SenderId",
                        column: x => x.SenderId,
                        principalSchema: "GitHub",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_UserId",
                table: "Issues",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueStatusChanges_IssueId",
                table: "IssueStatusChanges",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueEvents_IssueId",
                schema: "GitHub",
                table: "IssueEvents",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueEvents_RepositoryId",
                schema: "GitHub",
                table: "IssueEvents",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueEvents_SenderId",
                schema: "GitHub",
                table: "IssueEvents",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_GitHubIssueInternalId",
                schema: "GitHub",
                table: "Labels",
                column: "GitHubIssueInternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_OwnerId",
                schema: "GitHub",
                table: "Repositories",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssueStatusChanges");

            migrationBuilder.DropTable(
                name: "IssueEvents",
                schema: "GitHub");

            migrationBuilder.DropTable(
                name: "Labels",
                schema: "GitHub");

            migrationBuilder.DropTable(
                name: "Repositories",
                schema: "GitHub");

            migrationBuilder.DropTable(
                name: "Issues");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "GitHub");
        }
    }
}
