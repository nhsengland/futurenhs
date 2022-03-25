CREATE TABLE [dbo].[DiscussionTag] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Tag]                NVARCHAR (100)   NOT NULL,
    [Slug]               NVARCHAR (100)   NOT NULL,
    [Description]        NVARCHAR (MAX)   NULL,
    [ExtendedDataString] NVARCHAR (MAX)   NULL,
    [IsDeleted]          BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]         ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.DiscussionTag] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tag_Slug]
    ON [dbo].[DiscussionTag]([Slug] ASC);

