CREATE TABLE [dbo].[Like] (
    [Id]                       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Amount]                   SMALLINT              NOT NULL,
    [CreatedAtUTC]             DATETIME2         NOT NULL,
    [Comment_Id]               UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id]        UNIQUEIDENTIFIER NOT NULL,
    [LikedByMembershipUser_Id] UNIQUEIDENTIFIER NULL,
    [RowVersion]               ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.Vote] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Vote_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Vote_dbo.MembershipUser_VotedByMembershipUser_Id] FOREIGN KEY ([LikedByMembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Vote_dbo.Comment_Comment_Id] FOREIGN KEY ([Comment_Id]) REFERENCES [dbo].[Comment] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Like]([MembershipUser_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Comment_Id]
    ON [dbo].[Like]([Comment_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_VotedByMembershipUser_Id]
    ON [dbo].[Like]([LikedByMembershipUser_Id] ASC);

