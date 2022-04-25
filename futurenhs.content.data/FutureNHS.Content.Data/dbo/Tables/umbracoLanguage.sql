CREATE TABLE [dbo].[umbracoLanguage] (
    [id]                   INT            IDENTITY (1, 1) NOT NULL,
    [languageISOCode]      NVARCHAR (14)  NULL,
    [languageCultureName]  NVARCHAR (100) NULL,
    [isDefaultVariantLang] BIT            CONSTRAINT [DF_umbracoLanguage_isDefaultVariantLang] DEFAULT ('0') NOT NULL,
    [mandatory]            BIT            CONSTRAINT [DF_umbracoLanguage_mandatory] DEFAULT ('0') NOT NULL,
    [fallbackLanguageId]   INT            NULL,
    CONSTRAINT [PK_umbracoLanguage] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoLanguage_umbracoLanguage_id] FOREIGN KEY ([fallbackLanguageId]) REFERENCES [dbo].[umbracoLanguage] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoLanguage_languageISOCode]
    ON [dbo].[umbracoLanguage]([languageISOCode] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoLanguage_fallbackLanguageId]
    ON [dbo].[umbracoLanguage]([fallbackLanguageId] ASC);

