CREATE TABLE [dbo].[TagNotification] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [TopicTag_Id]       UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.TagNotification] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.TagNotification_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.TagNotification_dbo.TopicTag_TopicTag_Id] FOREIGN KEY ([TopicTag_Id]) REFERENCES [dbo].[TopicTag] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[TagNotification]([MembershipUser_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TopicTag_Id]
    ON [dbo].[TagNotification]([TopicTag_Id] ASC);

