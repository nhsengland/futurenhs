CREATE TABLE [dbo].[umbracoContent] (
    [nodeId]        INT NOT NULL,
    [contentTypeId] INT NOT NULL,
    CONSTRAINT [PK_umbracoContent] PRIMARY KEY CLUSTERED ([nodeId] ASC),
    CONSTRAINT [FK_umbracoContent_cmsContentType_NodeId] FOREIGN KEY ([contentTypeId]) REFERENCES [dbo].[cmsContentType] ([nodeId]),
    CONSTRAINT [FK_umbracoContent_umbracoNode_id] FOREIGN KEY ([nodeId]) REFERENCES [dbo].[umbracoNode] ([id])
);

