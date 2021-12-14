CREATE TABLE [dbo].[PollVote] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [PollAnswer_Id]     UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.PollVote] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.PollVote_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.PollVote_dbo.PollAnswer_PollAnswer_Id] FOREIGN KEY ([PollAnswer_Id]) REFERENCES [dbo].[PollAnswer] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[PollVote]([MembershipUser_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PollAnswer_Id]
    ON [dbo].[PollVote]([PollAnswer_Id] ASC);

