CREATE TABLE [dbo].[Group] (
    [Id]                   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Name]                 NVARCHAR (255)   NOT NULL,
    [Description]          NVARCHAR (4000)  NULL,
    [IsLocked]             BIT              DEFAULT ((0)) NOT NULL,
    [ModerateDiscussions]  BIT              DEFAULT ((0)) NOT NULL,
    [ModerateComments]     BIT              DEFAULT ((0)) NOT NULL,
    [SortOrder]            INT              DEFAULT ((0)) NOT NULL,
    [CreatedAtUtc]         DATETIME2        NOT NULL,
    [Slug]                 NVARCHAR (450)   NOT NULL,
    [PageTitle]            NVARCHAR (80)    NULL,
    [Path]                 NVARCHAR (2500)  NULL,
    [MetaDescription]      NVARCHAR (200)   NULL,
    [Colour]               NVARCHAR (50)    NULL,
    [Image]                NVARCHAR (200)   NULL,
    [Parent_GroupId]       UNIQUEIDENTIFIER NULL,
    [Section_Id]           UNIQUEIDENTIFIER NULL,
    [PublicGroup]          BIT              DEFAULT ((0)) NOT NULL,
    [HiddenGroup]          BIT              DEFAULT ((0)) NOT NULL,
    [GroupOwner]           UNIQUEIDENTIFIER NULL,
    [Subtitle]             NVARCHAR (255)   NULL,
    [Introduction]         NVARCHAR (4000)  DEFAULT ('') NOT NULL,
    [AboutUs]              NVARCHAR (4000)  NULL,
    [ImageId]              UNIQUEIDENTIFIER NULL,
    [ThemeId]              UNIQUEIDENTIFIER NULL, 
    [IsDeleted]            BIT              DEFAULT ((0)) NOT NULL,
    [RowVersion]           ROWVERSION       NOT NULL,
    [CreatedBy]            UNIQUEIDENTIFIER NULL,
    [ModifiedBy]           UNIQUEIDENTIFIER NULL,
    [ModifiedAtUtc]        DATETIME2 (7)    NULL,
    CONSTRAINT [PK_dbo.Group] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Group_dbo.Group_Group_Id] FOREIGN KEY ([Parent_GroupId]) REFERENCES [dbo].[Group] ([Id]),
    CONSTRAINT [FK_dbo.Group_dbo.Image_HeaderImage] FOREIGN KEY ([ImageId]) REFERENCES [dbo].[Image] ([Id]),
    CONSTRAINT [FK_dbo.Group_dbo.GroupOwner_MembershipUser_Id] FOREIGN KEY ([GroupOwner]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Group_dbo.Section_Section_Id] FOREIGN KEY ([Section_Id]) REFERENCES [dbo].[Section] ([Id]),
    CONSTRAINT [FK_dbo.Group_dbo.MembershipUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Group_dbo.MembershipUser_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[MembershipUser] ([Id])

);


GO
CREATE NONCLUSTERED INDEX [IX_Group_Id]
    ON [dbo].[Group]([Parent_GroupId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Group_Slug]
    ON [dbo].[Group]([Slug] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Group]([GroupOwner] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Section_Id]
    ON [dbo].[Group]([Section_Id] ASC);