CREATE TABLE [dbo].[umbracoContentVersionCultureVariation] (
    [id]              INT            IDENTITY (1, 1) NOT NULL,
    [versionId]       INT            NOT NULL,
    [languageId]      INT            NOT NULL,
    [name]            NVARCHAR (255) NOT NULL,
    [date]            DATETIME       NOT NULL,
    [availableUserId] INT            NULL,
    CONSTRAINT [PK_umbracoContentVersionCultureVariation] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoContentVersionCultureVariation_umbracoContentVersion_id] FOREIGN KEY ([versionId]) REFERENCES [dbo].[umbracoContentVersion] ([id]),
    CONSTRAINT [FK_umbracoContentVersionCultureVariation_umbracoLanguage_id] FOREIGN KEY ([languageId]) REFERENCES [dbo].[umbracoLanguage] ([id]),
    CONSTRAINT [FK_umbracoContentVersionCultureVariation_umbracoUser_id] FOREIGN KEY ([availableUserId]) REFERENCES [dbo].[umbracoUser] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoContentVersionCultureVariation_VersionId]
    ON [dbo].[umbracoContentVersionCultureVariation]([versionId] ASC, [languageId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoContentVersionCultureVariation_LanguageId]
    ON [dbo].[umbracoContentVersionCultureVariation]([languageId] ASC);

