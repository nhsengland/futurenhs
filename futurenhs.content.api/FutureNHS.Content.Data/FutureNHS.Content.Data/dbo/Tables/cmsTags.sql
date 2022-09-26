CREATE TABLE [dbo].[cmsTags] (
    [id]         INT            IDENTITY (1, 1) NOT NULL,
    [group]      NVARCHAR (100) NOT NULL,
    [languageId] INT            NULL,
    [tag]        NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_cmsTags] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_cmsTags_umbracoLanguage_id] FOREIGN KEY ([languageId]) REFERENCES [dbo].[umbracoLanguage] ([id])
);


GO
CREATE NONCLUSTERED INDEX [IX_cmsTags_LanguageId]
    ON [dbo].[cmsTags]([languageId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_cmsTags]
    ON [dbo].[cmsTags]([group] ASC, [tag] ASC, [languageId] ASC);

