CREATE DATABASE CoeIdeas;
GO
CREATE LOGIN [OctavaService] WITH PASSWORD=N'P@ssw0rd', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
USE [CoeIdeas]
GO
CREATE USER [OctavaService] FOR LOGIN [OctavaService]
GO
ALTER ROLE [db_datareader] ADD MEMBER [OctavaService]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [OctavaService]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [OctavaService]
GO

CREATE DATABASE ServiceBusEmulator;
GO
USE [ServiceBusEmulator]
GO
CREATE USER [OctavaService] FOR LOGIN [OctavaService]
GO
ALTER ROLE [db_datareader] ADD MEMBER [OctavaService]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [OctavaService]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [OctavaService]
GO


-- The rest is generated from EntityFrameworkCore:
IF OBJECT_ID(N'__EFMigrationsHistory') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [MessageProperties] (
    [Id] int NOT NULL IDENTITY,
    [Key] nvarchar(128) NULL,
    [MessageId] uniqueidentifier NOT NULL,
    [Value] nvarchar(1024) NULL,
    [ValueType] nvarchar(1024) NULL,
    CONSTRAINT [PK_MessageProperties] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Messages] (
    [Id] uniqueidentifier NOT NULL,
    [CreatedDateUtc] datetime2 NOT NULL,
    [Label] nvarchar(128) NULL,
    [LockToken] nvarchar(max) NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([Id])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180320141041_Initial', N'2.0.2-rtm-10011');

GO



USE [CoeIdeas]
GO


IF OBJECT_ID(N'__EFMigrationsHistory') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Initiatives] (
    [Id] int NOT NULL IDENTITY,
    [AssigneeId] int NULL,
    [BusinessCaseUrl] nvarchar(2048) NULL,
    [CreatedDate] datetimeoffset NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [InvestmentRequestFormUrl] nvarchar(2048) NULL,
    [Status] int NOT NULL,
    [Title] nvarchar(255) NOT NULL,
    [Uid] uniqueidentifier NOT NULL,
    [WorkOrderId] nvarchar(128) NULL,
    CONSTRAINT [PK_Initiatives] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [InitiativeStatusHistories] (
    [Id] int NOT NULL IDENTITY,
    [InitiativeId] uniqueidentifier NOT NULL,
    [PersonId] int NULL,
    [Status] int NOT NULL,
    [StatusEntryDateUtc] datetime2 NOT NULL,
    [Text] nvarchar(1024) NULL,
    CONSTRAINT [PK_InitiativeStatusHistories] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [StringTemplates] (
    [Id] int NOT NULL IDENTITY,
    [Category] int NOT NULL,
    [Key] nvarchar(64) NULL,
    [Text] nvarchar(2048) NULL,
    CONSTRAINT [PK_StringTemplates] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Stakeholder] (
    [Id] int NOT NULL IDENTITY,
    [InitiativeId] int NULL,
    [PersonId] int NOT NULL,
    [Type] int NOT NULL,
    CONSTRAINT [PK_Stakeholder] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Stakeholder_Initiatives_InitiativeId] FOREIGN KEY ([InitiativeId]) REFERENCES [Initiatives] ([Id]) ON DELETE NO ACTION
);

GO

CREATE UNIQUE INDEX [IX_Initiatives_WorkOrderId] ON [Initiatives] ([WorkOrderId]) WHERE [WorkOrderId] IS NOT NULL;

GO

CREATE INDEX [IX_Stakeholder_InitiativeId] ON [Stakeholder] ([InitiativeId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180315203805_Initial', N'2.0.2-rtm-10011');

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180315203850_Auditing', N'2.0.2-rtm-10011');

GO


INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Submit_present', 'Thank you! Your initiative has been submitted and will be assigned for review. An OCT representative will contact you by {0}')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Submit_past', 'Thank you! Your initiative was submitted.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Review_present', 'Your initiative has been assigned for review. {0} will be working with you to further define your initiative.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Review_past', 'Your initiative has been assigned and reviewed.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Collaborate_present', '{0} is actively working with you to complete an Investment Request for your initiative so that it may be prioritized and approved.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Collaborate_past', 'An Investment Request has been completed for your initiative.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Deliver_present', 'Your request has been submitted to Technology Investment for Business Technology Steering Committee Approval. A Technology Investment representative will notify you when your initiative has been approved.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Deliver_past', 'Your initiative has been successfully submitted as a project with Solutions Delivery.')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Cancelled_present', 'Your request has been cancelled. ')
INSERT INTO StringTemplates(Category, [Key], [Text]) VALUES(1, 'Cancelled_past', 'Your request was cancelled')

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180315203940_StatusHistoryStringTemplates', N'2.0.2-rtm-10011');

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'InitiativeStatusHistories') AND [c].[name] = N'Text');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [InitiativeStatusHistories] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [InitiativeStatusHistories] DROP COLUMN [Text];

GO

ALTER TABLE [InitiativeStatusHistories] ADD [ExpectedExitDateUtc] datetime2 NULL;

GO

CREATE TABLE [StatusEtas] (
    [Id] int NOT NULL IDENTITY,
    [EtaType] int NOT NULL,
    [Status] int NOT NULL,
    [Time] int NOT NULL,
    CONSTRAINT [PK_StatusEtas] PRIMARY KEY ([Id])
);

GO

INSERT INTO [dbo].[StatusEtas] ([EtaType],[Status],[Time]) VALUES(2,3,14400)

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180323203736_AddedStatusEtas', N'2.0.2-rtm-10011');

GO

ALTER TABLE [InitiativeStatusHistories] ADD [StatusDescriptionOverride] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180419152741_OverridableStatusDescriptions', N'2.0.2-rtm-10011');

GO

IF OBJECT_ID(N'__EFMigrationsHistory') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [PermissionRoles] (
    [Id] int NOT NULL IDENTITY,
    [Permssion] nvarchar(max) NULL,
    [Role] nvarchar(max) NULL,
    CONSTRAINT [PK_PermissionRoles] PRIMARY KEY ([Id])
);

GO

INSERT INTO [dbo].[PermissionRoles]([Permssion],[Role]) VALUES ('UpdateStatusDescription','Octava Business Analyst')

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180424031308_Permissions', N'2.0.2-rtm-10011');

GO



ALTER TABLE [dbo].[InitiativeStatusHistories] DROP COLUMN InitiativeId;

ALTER TABLE [dbo].[InitiativeStatusHistories] ADD InitiativeId INT NOT NULL;

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'InitiativeStatusHistories') AND [c].[name] = N'InitiativeId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [InitiativeStatusHistories] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [InitiativeStatusHistories] ALTER COLUMN [InitiativeId] int NULL;

GO

ALTER TABLE [Initiatives] ADD [ApexId] int NOT NULL DEFAULT 0;

GO

CREATE INDEX [IX_InitiativeStatusHistories_InitiativeId] ON [InitiativeStatusHistories] ([InitiativeId]);

GO

ALTER TABLE [InitiativeStatusHistories] ADD CONSTRAINT [FK_InitiativeStatusHistories_Initiatives_InitiativeId] FOREIGN KEY ([InitiativeId]) REFERENCES [Initiatives] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180426212005_AddApexId', N'2.0.2-rtm-10011');

GO

SET IDENTITY_INSERT [dbo].[Initiatives] ON 
GO
INSERT [dbo].[Initiatives] ([Id], [AssigneeId], [BusinessCaseUrl], [CreatedDate], [Description], [InvestmentRequestFormUrl], [Status], [Title], [Uid], [WorkOrderId]) VALUES (20, 33, NULL, CAST(N'2018-04-26T21:49:30.5515831+00:00' AS DateTimeOffset), N'There was initially a concern over the networking ability of the computer system at the funicular site.  Through discussions with IT Networking this has been resolved and is no longer an issue.', NULL, 5, N' Funicular - System implementation ', N'1f5f98fd-3c4e-4c2b-9245-5b4a9688efbf', N'AGGCMB6PF7AHNAPHT109PGWOGC8EBH')
GO
SET IDENTITY_INSERT [dbo].[Initiatives] OFF
GO
SET IDENTITY_INSERT [dbo].[Stakeholder] ON 
GO
INSERT [dbo].[Stakeholder] ([Id], [InitiativeId], [PersonId], [Type]) VALUES (20, 20, 3, 0)
GO
SET IDENTITY_INSERT [dbo].[Stakeholder] OFF
GO
SET IDENTITY_INSERT [dbo].[InitiativeStatusHistories] ON 
GO
INSERT [dbo].[InitiativeStatusHistories] ([Id], [PersonId], [Status], [StatusEntryDateUtc], [ExpectedExitDateUtc], [StatusDescriptionOverride], [InitiativeId]) VALUES (7, NULL, 3, CAST(N'2018-04-26T21:49:32.2059619' AS DateTime2), CAST(N'2018-04-27T15:49:32.0430000' AS DateTime2), NULL, 20)
GO
INSERT [dbo].[InitiativeStatusHistories] ([Id], [PersonId], [Status], [StatusEntryDateUtc], [ExpectedExitDateUtc], [StatusDescriptionOverride], [InitiativeId]) VALUES (8, 33, 4, CAST(N'2018-04-26T21:51:17.8431642' AS DateTime2), NULL, NULL, 20)
GO
INSERT [dbo].[InitiativeStatusHistories] ([Id], [PersonId], [Status], [StatusEntryDateUtc], [ExpectedExitDateUtc], [StatusDescriptionOverride], [InitiativeId]) VALUES (9, 33, 5, CAST(N'2018-04-26T21:53:21.1775655' AS DateTime2), NULL, NULL, 20)
GO
SET IDENTITY_INSERT [dbo].[InitiativeStatusHistories] OFF
GO