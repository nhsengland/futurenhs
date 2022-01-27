CREATE TABLE [dbo].[Topic_Tag] (
    [Topic_Id]    UNIQUEIDENTIFIER NOT NULL,
    [TopicTag_Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.Topic_Tag] PRIMARY KEY CLUSTERED ([Topic_Id] ASC, [TopicTag_Id] ASC),
    CONSTRAINT [FK_dbo.Topic_Tag_dbo.Topic_Topic_Id] FOREIGN KEY ([Topic_Id]) REFERENCES [dbo].[Topic] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.Topic_Tag_dbo.TopicTag_TopicTag_Id] FOREIGN KEY ([TopicTag_Id]) REFERENCES [dbo].[TopicTag] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Topic_Id]
    ON [dbo].[Topic_Tag]([Topic_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TopicTag_Id]
    ON [dbo].[Topic_Tag]([TopicTag_Id] ASC);

