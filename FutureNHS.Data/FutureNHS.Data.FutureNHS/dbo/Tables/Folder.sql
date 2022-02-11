CREATE TABLE [dbo].[Folder] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [Name]         NVARCHAR (200)  NOT NULL,
    [Description]  NVARCHAR (4000)  NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedAtUtc] DATETIME2 (7)    NOT NULL,
    [ModifiedBy]      UNIQUEIDENTIFIER NULL,
    [ModifiedAtUtc] DATETIME2 (7)    NULL,
    [ParentFolder] UNIQUEIDENTIFIER NULL,
    [FileCount]    INT              DEFAULT ((0)) NOT NULL,
    [Group_Id]     UNIQUEIDENTIFIER NOT NULL,
    [IsDeleted]    BIT              DEFAULT ((0)) NOT NULL,
    [RowVersion]   ROWVERSION       NOT NULL,
    CONSTRAINT [PK_dbo.Folder] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Folder_dbo.Folder_ParentFolder] FOREIGN KEY ([ParentFolder]) REFERENCES [dbo].[Folder] ([Id]),
    CONSTRAINT [FK_dbo.Folder_dbo.Group_Group_Id] FOREIGN KEY ([Group_Id]) REFERENCES [dbo].[Group] ([Id]),
    CONSTRAINT [FK_dbo.Folder_dbo.MembershipUser_AddedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Folder_dbo.MembershipUser_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[MembershipUser] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ParentFolder]
    ON [dbo].[Folder]([ParentFolder] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ParentGroup]
    ON [dbo].[Folder]([Group_Id] ASC);

