CREATE TABLE [dbo].[Discussion] (
    [Entity_Id]          UNIQUEIDENTIFIER NOT NULL,
    [Title]              NVARCHAR (200)   NOT NULL,
    [CreatedAtUtc]       DATETIME2         NOT NULL,
    [CreatedBy]          UNIQUEIDENTIFIER NOT NULL,
    [ModifiedAtUtc]      DATETIME2 NULL, 
    [ModifiedBy]         UNIQUEIDENTIFIER NULL, 
    [Views]              INT              NOT NULL DEFAULT ((0)),
    [IsSticky]           BIT              NOT NULL,
    [IsLocked]           BIT              NOT NULL,
    [Group_Id]           UNIQUEIDENTIFIER NOT NULL,
    [Poll_Id]            UNIQUEIDENTIFIER NULL,
    [Category_Id]        UNIQUEIDENTIFIER NULL,
    [Content]            NVARCHAR(MAX) NOT NULL, 
    [IsDeleted]          BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion] ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.Discussion] PRIMARY KEY CLUSTERED ([Entity_Id] ASC),
    CONSTRAINT [FK_dbo.Discussion_dbo.Entity_Id] FOREIGN KEY ([Entity_Id]) REFERENCES [dbo].[Entity] ([Id]),
    CONSTRAINT [FK_dbo.Discussion_dbo.Category_Category_Id] FOREIGN KEY ([Category_Id]) REFERENCES [dbo].[Category] ([Id]),
    CONSTRAINT [FK_dbo.Discussion_dbo.Group_Group_Id] FOREIGN KEY ([Group_Id]) REFERENCES [dbo].[Group] ([Id]),
    CONSTRAINT [FK_dbo.Discussion_dbo.MembershipUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Discussion_dbo.Poll_Poll_Id] FOREIGN KEY ([Poll_Id]) REFERENCES [dbo].[Poll] ([Id]),
    CONSTRAINT [FK_dbo.Discussion_dbo.MembershipUser_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
);

GO
CREATE NONCLUSTERED INDEX [IX_Category_Id]
    ON [dbo].[Discussion]([Category_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Group_Id]
    ON [dbo].[Discussion]([Group_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Discussion]([CreatedBy] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Poll_Id]
    ON [dbo].[Discussion]([Poll_Id] ASC);

