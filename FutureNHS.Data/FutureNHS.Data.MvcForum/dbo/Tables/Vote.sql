CREATE TABLE [dbo].[Vote] (
    [Id]                       UNIQUEIDENTIFIER NOT NULL,
    [Amount]                   INT              NOT NULL,
    [DateVoted]                DATETIME         NULL,
    [Post_Id]                  UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id]        UNIQUEIDENTIFIER NOT NULL,
    [VotedByMembershipUser_Id] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_dbo.Vote] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Vote_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Vote_dbo.MembershipUser_VotedByMembershipUser_Id] FOREIGN KEY ([VotedByMembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Vote_dbo.Post_Post_Id] FOREIGN KEY ([Post_Id]) REFERENCES [dbo].[Post] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Vote]([MembershipUser_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Post_Id]
    ON [dbo].[Vote]([Post_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_VotedByMembershipUser_Id]
    ON [dbo].[Vote]([VotedByMembershipUser_Id] ASC);

