CREATE TABLE [dbo].[Entity_Like] (
    [Entity_Id]                UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id]        UNIQUEIDENTIFIER NOT NULL,
    [CreatedAtUTC]             DATETIME2         NOT NULL,   
    [RowVersion]               ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.Vote] PRIMARY KEY CLUSTERED ([Entity_Id], [MembershipUser_Id] ASC),
    CONSTRAINT [FK_dbo.Vote_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Vote_dbo.MembershipUser_VotedByMembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Vote_dbo.Entity_Id] FOREIGN KEY ([Entity_Id]) REFERENCES [dbo].[Entity] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Entity_Like]([MembershipUser_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Entity_Id]
    ON [dbo].[Entity_Like]([Entity_Id] ASC);

