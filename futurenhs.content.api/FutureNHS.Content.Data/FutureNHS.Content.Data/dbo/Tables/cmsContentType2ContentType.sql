CREATE TABLE [dbo].[cmsContentType2ContentType] (
    [parentContentTypeId] INT NOT NULL,
    [childContentTypeId]  INT NOT NULL,
    CONSTRAINT [PK_cmsContentType2ContentType] PRIMARY KEY CLUSTERED ([parentContentTypeId] ASC, [childContentTypeId] ASC),
    CONSTRAINT [FK_cmsContentType2ContentType_umbracoNode_child] FOREIGN KEY ([childContentTypeId]) REFERENCES [dbo].[umbracoNode] ([id]),
    CONSTRAINT [FK_cmsContentType2ContentType_umbracoNode_parent] FOREIGN KEY ([parentContentTypeId]) REFERENCES [dbo].[umbracoNode] ([id])
);

