CREATE TABLE [dbo].[Group] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (450)   NOT NULL,
    [Description]        NVARCHAR (4000)   NULL,
    [IsLocked]           BIT              NOT NULL,
    [ModerateTopics]     BIT              NOT NULL,
    [ModeratePosts]      BIT              NOT NULL,
    [SortOrder]          INT              NOT NULL,
    [DateCreated]        DATETIME         NOT NULL,
    [Slug]               NVARCHAR (450)   NOT NULL,
    [PageTitle]          NVARCHAR (80)    NULL,
    [Path]               NVARCHAR (2500)  NULL,
    [MetaDescription]    NVARCHAR (200)   NULL,
    [Colour]             NVARCHAR (50)    NULL,
    [Image]              NVARCHAR (200)   NULL,
    [ExtendedDataString] NVARCHAR (MAX)   NULL,
    [Group_Id]           UNIQUEIDENTIFIER NULL,
    [Section_Id]         UNIQUEIDENTIFIER NULL,
    [PublicGroup]        BIT              DEFAULT ((0)) NOT NULL,
    [HiddenGroup]        BIT              DEFAULT ((0)) NOT NULL,
    [IsDeleted]          BIT              DEFAULT ((0)) NOT NULL,
    [RowVersion]         ROWVERSION       NOT NULL,
    [MembershipUser_Id]  UNIQUEIDENTIFIER NULL,
    [Subtitle]           NVARCHAR (254)   NULL,
    [Introduction]       NVARCHAR (4000)  DEFAULT ('') NOT NULL,
    [AboutUs]            NVARCHAR (4000)  NULL,
    [HeaderImage]        UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_dbo.Group] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Group_dbo.Group_Group_Id] FOREIGN KEY ([Group_Id]) REFERENCES [dbo].[Group] ([Id]),
    CONSTRAINT [FK_dbo.Group_dbo.Image_HeaderImage] FOREIGN KEY ([HeaderImage]) REFERENCES [dbo].[Image] ([Id]),
    CONSTRAINT [FK_dbo.Group_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Group_dbo.Section_Section_Id] FOREIGN KEY ([Section_Id]) REFERENCES [dbo].[Section] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Group_Id]
    ON [dbo].[Group]([Group_Id] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Group_Slug]
    ON [dbo].[Group]([Slug] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Group]([MembershipUser_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Section_Id]
    ON [dbo].[Group]([Section_Id] ASC);

