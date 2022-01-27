CREATE TABLE [dbo].[TopicNotification] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [Topic_Id]          UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.TopicNotification] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.TopicNotification_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.TopicNotification_dbo.Topic_Topic_Id] FOREIGN KEY ([Topic_Id]) REFERENCES [dbo].[Topic] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[TopicNotification]([MembershipUser_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Topic_Id]
    ON [dbo].[TopicNotification]([Topic_Id] ASC);

