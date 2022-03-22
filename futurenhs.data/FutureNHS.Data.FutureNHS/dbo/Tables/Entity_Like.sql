CREATE TABLE [dbo].[Entity_Like] (
    [Comment_Id]                UNIQUEIDENTIFIER NOT NULL,
    [Entity_Id]                UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id]        UNIQUEIDENTIFIER NOT NULL,
    [CreatedAtUTC]             DATETIME2         NOT NULL,   
    [RowVersion]               ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.Entity_Like] PRIMARY KEY CLUSTERED ([Comment_Id], [MembershipUser_Id] ASC),
    CONSTRAINT [FK_dbo.Entity_Like_dbo.MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Entity_Like_dbo.Entity_Comment_Id] FOREIGN KEY ([Comment_Id], [Entity_Id]) REFERENCES [dbo].[Entity_Comment] ([Id], [Entity_Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_Entity_Like_MembershipUser_Id]
    ON [dbo].[Entity_Like]([MembershipUser_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Entity_Like_Comment_Id]
    ON [dbo].[Entity_Like]([Comment_Id] ASC);

