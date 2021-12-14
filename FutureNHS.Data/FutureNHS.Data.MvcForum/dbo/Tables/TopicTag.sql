CREATE TABLE [dbo].[TopicTag] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [Tag]                NVARCHAR (100)   NOT NULL,
    [Slug]               NVARCHAR (100)   NOT NULL,
    [Description]        NVARCHAR (MAX)   NULL,
    [ExtendedDataString] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_dbo.TopicTag] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tag_Slug]
    ON [dbo].[TopicTag]([Slug] ASC);

