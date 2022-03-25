CREATE TABLE [dbo].[Discussion_Tag] (
    [Discussion_Id]    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [DiscussionTag_Id] UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]         ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.Discussion_Tag] PRIMARY KEY CLUSTERED ([Discussion_Id] ASC, [DiscussionTag_Id] ASC),
    CONSTRAINT [FK_dbo.Discussion_Tag_dbo.Discussion_Discussion_Id] FOREIGN KEY ([Discussion_Id]) REFERENCES [dbo].[Discussion] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.Discussion_Tag_dbo.DiscussionTag_DiscussionTag_Id] FOREIGN KEY ([DiscussionTag_Id]) REFERENCES [dbo].[DiscussionTag] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Discussion_Id]
    ON [dbo].[Discussion_Tag]([Discussion_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DiscussionTag_Id]
    ON [dbo].[Discussion_Tag]([DiscussionTag_Id] ASC);

