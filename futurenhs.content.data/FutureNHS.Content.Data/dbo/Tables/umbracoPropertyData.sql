CREATE TABLE [dbo].[umbracoPropertyData] (
    [id]             INT             IDENTITY (1, 1) NOT NULL,
    [versionId]      INT             NOT NULL,
    [propertyTypeId] INT             NOT NULL,
    [languageId]     INT             NULL,
    [segment]        NVARCHAR (256)  NULL,
    [intValue]       INT             NULL,
    [decimalValue]   DECIMAL (38, 6) NULL,
    [dateValue]      DATETIME        NULL,
    [varcharValue]   NVARCHAR (512)  NULL,
    [textValue]      NTEXT           NULL,
    CONSTRAINT [PK_umbracoPropertyData] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoPropertyData_cmsPropertyType_id] FOREIGN KEY ([propertyTypeId]) REFERENCES [dbo].[cmsPropertyType] ([id]),
    CONSTRAINT [FK_umbracoPropertyData_umbracoContentVersion_id] FOREIGN KEY ([versionId]) REFERENCES [dbo].[umbracoContentVersion] ([id]),
    CONSTRAINT [FK_umbracoPropertyData_umbracoLanguage_id] FOREIGN KEY ([languageId]) REFERENCES [dbo].[umbracoLanguage] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoPropertyData_VersionId]
    ON [dbo].[umbracoPropertyData]([versionId] ASC, [propertyTypeId] ASC, [languageId] ASC, [segment] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoPropertyData_PropertyTypeId]
    ON [dbo].[umbracoPropertyData]([propertyTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoPropertyData_LanguageId]
    ON [dbo].[umbracoPropertyData]([languageId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoPropertyData_Segment]
    ON [dbo].[umbracoPropertyData]([segment] ASC);

