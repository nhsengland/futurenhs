CREATE TABLE [dbo].[Folder] (
    [Description]  NVARCHAR (4000)  NULL,
    [AddedBy]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedAtUtc] DATETIME2 (7)    NOT NULL,
    [Id]           UNIQUEIDENTIFIER DEFAULT ('00000000-0000-0000-0000-000000000000') NOT NULL,
    [Name]         NVARCHAR (1000)  NOT NULL,
    [ParentFolder] UNIQUEIDENTIFIER NULL,
    [FileCount]    INT              DEFAULT ((0)) NOT NULL,
    [ParentGroup]  UNIQUEIDENTIFIER DEFAULT ('00000000-0000-0000-0000-000000000000') NOT NULL,
    [IsDeleted]    BIT              DEFAULT ((0)) NOT NULL,
    [RowVersion]   ROWVERSION       NOT NULL,
    CONSTRAINT [PK_dbo.Folder] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Folder_dbo.Folder_ParentFolder] FOREIGN KEY ([ParentFolder]) REFERENCES [dbo].[Folder] ([Id]),
    CONSTRAINT [FK_dbo.Folder_dbo.Group_ParentGroup] FOREIGN KEY ([ParentGroup]) REFERENCES [dbo].[Group] ([Id]),
    CONSTRAINT [FK_dbo.Folder_dbo.MembershipUser_AddedBy] FOREIGN KEY ([AddedBy]) REFERENCES [dbo].[MembershipUser] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ParentFolder]
    ON [dbo].[Folder]([ParentFolder] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ParentGroup]
    ON [dbo].[Folder]([ParentGroup] ASC);

