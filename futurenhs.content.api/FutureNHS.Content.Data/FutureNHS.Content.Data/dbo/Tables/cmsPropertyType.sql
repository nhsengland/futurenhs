CREATE TABLE [dbo].[cmsPropertyType] (
    [id]                      INT              IDENTITY (1, 1) NOT NULL,
    [dataTypeId]              INT              NOT NULL,
    [contentTypeId]           INT              NOT NULL,
    [propertyTypeGroupId]     INT              NULL,
    [Alias]                   NVARCHAR (255)   NOT NULL,
    [Name]                    NVARCHAR (255)   NULL,
    [sortOrder]               INT              CONSTRAINT [DF_cmsPropertyType_sortOrder] DEFAULT ('0') NOT NULL,
    [mandatory]               BIT              CONSTRAINT [DF_cmsPropertyType_mandatory] DEFAULT ('0') NOT NULL,
    [mandatoryMessage]        NVARCHAR (500)   NULL,
    [validationRegExp]        NVARCHAR (255)   NULL,
    [validationRegExpMessage] NVARCHAR (500)   NULL,
    [Description]             NVARCHAR (2000)  NULL,
    [labelOnTop]              BIT              CONSTRAINT [DF_cmsPropertyType_labelOnTop] DEFAULT ('0') NOT NULL,
    [variations]              INT              CONSTRAINT [DF_cmsPropertyType_variations] DEFAULT ('1') NOT NULL,
    [UniqueID]                UNIQUEIDENTIFIER CONSTRAINT [DF_cmsPropertyType_UniqueID] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_cmsPropertyType] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_cmsPropertyType_cmsContentType_nodeId] FOREIGN KEY ([contentTypeId]) REFERENCES [dbo].[cmsContentType] ([nodeId]),
    CONSTRAINT [FK_cmsPropertyType_cmsPropertyTypeGroup_id] FOREIGN KEY ([propertyTypeGroupId]) REFERENCES [dbo].[cmsPropertyTypeGroup] ([id]),
    CONSTRAINT [FK_cmsPropertyType_umbracoDataType_nodeId] FOREIGN KEY ([dataTypeId]) REFERENCES [dbo].[umbracoDataType] ([nodeId])
);


GO
CREATE NONCLUSTERED INDEX [IX_cmsPropertyTypeAlias]
    ON [dbo].[cmsPropertyType]([Alias] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_cmsPropertyTypeUniqueID]
    ON [dbo].[cmsPropertyType]([UniqueID] ASC);

