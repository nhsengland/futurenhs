CREATE TABLE [dbo].[cmsLanguageText] (
    [pk]         INT              IDENTITY (1, 1) NOT NULL,
    [languageId] INT              NOT NULL,
    [UniqueId]   UNIQUEIDENTIFIER NOT NULL,
    [value]      NVARCHAR (1000)  NOT NULL,
    CONSTRAINT [PK_cmsLanguageText] PRIMARY KEY CLUSTERED ([pk] ASC),
    CONSTRAINT [FK_cmsLanguageText_cmsDictionary_id] FOREIGN KEY ([UniqueId]) REFERENCES [dbo].[cmsDictionary] ([id]),
    CONSTRAINT [FK_cmsLanguageText_umbracoLanguage_id] FOREIGN KEY ([languageId]) REFERENCES [dbo].[umbracoLanguage] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_cmsLanguageText_languageId]
    ON [dbo].[cmsLanguageText]([languageId] ASC, [UniqueId] ASC);

