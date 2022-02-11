CREATE TABLE [dbo].[Favourite] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [CreatedAtUTC] DATETIME         NOT NULL,
    [MemberId]    UNIQUEIDENTIFIER NOT NULL,
    [CommentId]      UNIQUEIDENTIFIER NOT NULL,
    [DiscussionId]     UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.Favourite] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Favourite_dbo.MembershipUser_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Favourite_dbo.Comment_CommentId] FOREIGN KEY ([CommentId]) REFERENCES [dbo].[Comment] ([Id]),
    CONSTRAINT [FK_dbo.Favourite_dbo.Discussion_DiscussionId] FOREIGN KEY ([DiscussionId]) REFERENCES [dbo].[Discussion] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MemberId]
    ON [dbo].[Favourite]([MemberId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CommentId]
    ON [dbo].[Favourite]([CommentId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DiscussionId]
    ON [dbo].[Favourite]([DiscussionId] ASC);

