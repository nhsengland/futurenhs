CREATE TABLE [dbo].[cmsContentTypeAllowedContentType] (
    [Id]        INT NOT NULL,
    [AllowedId] INT NOT NULL,
    [SortOrder] INT CONSTRAINT [df_cmsContentTypeAllowedContentType_sortOrder] DEFAULT ('0') NOT NULL,
    CONSTRAINT [PK_cmsContentTypeAllowedContentType] PRIMARY KEY CLUSTERED ([Id] ASC, [AllowedId] ASC),
    CONSTRAINT [FK_cmsContentTypeAllowedContentType_cmsContentType] FOREIGN KEY ([Id]) REFERENCES [dbo].[cmsContentType] ([nodeId]),
    CONSTRAINT [FK_cmsContentTypeAllowedContentType_cmsContentType1] FOREIGN KEY ([AllowedId]) REFERENCES [dbo].[cmsContentType] ([nodeId])
);

