CREATE TABLE [dbo].[umbracoDocumentCultureVariation] (
    [id]         INT            IDENTITY (1, 1) NOT NULL,
    [nodeId]     INT            NOT NULL,
    [languageId] INT            NOT NULL,
    [edited]     BIT            NOT NULL,
    [available]  BIT            NOT NULL,
    [published]  BIT            NOT NULL,
    [name]       NVARCHAR (255) NULL,
    CONSTRAINT [PK_umbracoDocumentCultureVariation] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoDocumentCultureVariation_umbracoLanguage_id] FOREIGN KEY ([languageId]) REFERENCES [dbo].[umbracoLanguage] ([id]),
    CONSTRAINT [FK_umbracoDocumentCultureVariation_umbracoNode_id] FOREIGN KEY ([nodeId]) REFERENCES [dbo].[umbracoNode] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoDocumentCultureVariation_NodeId]
    ON [dbo].[umbracoDocumentCultureVariation]([nodeId] ASC, [languageId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoDocumentCultureVariation_LanguageId]
    ON [dbo].[umbracoDocumentCultureVariation]([languageId] ASC);

