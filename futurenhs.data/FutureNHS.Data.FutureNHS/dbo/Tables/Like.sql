CREATE TABLE [dbo].[Like] (
    [Id]                       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Amount]                   SMALLINT              NOT NULL,
    [CreatedAtUTC]             DATETIME2         NOT NULL,
    [Comment_Id]               UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]                UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]               ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.Vote] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Vote_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Vote_dbo.MembershipUser_VotedByMembershipUser_Id] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Vote_dbo.Comment_Comment_Id] FOREIGN KEY ([Comment_Id]) REFERENCES [dbo].[Comment] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Like]([CreatedBy] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Comment_Id]
    ON [dbo].[Like]([Comment_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_VotedByMembershipUser_Id]
    ON [dbo].[Like]([CreatedBy] ASC);

