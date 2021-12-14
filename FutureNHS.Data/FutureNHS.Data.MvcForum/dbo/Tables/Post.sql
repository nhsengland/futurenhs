CREATE TABLE [dbo].[Post] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [PostContent]        NVARCHAR (MAX)   NOT NULL,
    [DateCreated]        DATETIME         NOT NULL,
    [VoteCount]          INT              NOT NULL,
    [DateEdited]         DATETIME         NOT NULL,
    [IsSolution]         BIT              NOT NULL,
    [IsTopicStarter]     BIT              NULL,
    [FlaggedAsSpam]      BIT              NULL,
    [Pending]            BIT              NULL,
    [InReplyTo]          UNIQUEIDENTIFIER NULL,
    [ExtendedDataString] NVARCHAR (MAX)   NULL,
    [Topic_Id]           UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id]  UNIQUEIDENTIFIER NOT NULL,
    [ThreadId]           UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_dbo.Post] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Post_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Post_dbo.Topic_Topic_Id] FOREIGN KEY ([Topic_Id]) REFERENCES [dbo].[Topic] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Post]([MembershipUser_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Topic_Id]
    ON [dbo].[Post]([Topic_Id] ASC);

