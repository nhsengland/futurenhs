CREATE TABLE [dbo].[Favourite] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [DateCreated] DATETIME         NOT NULL,
    [MemberId]    UNIQUEIDENTIFIER NOT NULL,
    [PostId]      UNIQUEIDENTIFIER NOT NULL,
    [TopicId]     UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.Favourite] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Favourite_dbo.MembershipUser_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Favourite_dbo.Post_PostId] FOREIGN KEY ([PostId]) REFERENCES [dbo].[Post] ([Id]),
    CONSTRAINT [FK_dbo.Favourite_dbo.Topic_TopicId] FOREIGN KEY ([TopicId]) REFERENCES [dbo].[Topic] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MemberId]
    ON [dbo].[Favourite]([MemberId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PostId]
    ON [dbo].[Favourite]([PostId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TopicId]
    ON [dbo].[Favourite]([TopicId] ASC);

