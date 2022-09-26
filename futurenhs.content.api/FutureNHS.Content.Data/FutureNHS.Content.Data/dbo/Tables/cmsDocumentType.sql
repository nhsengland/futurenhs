CREATE TABLE [dbo].[cmsDocumentType] (
    [contentTypeNodeId] INT NOT NULL,
    [templateNodeId]    INT NOT NULL,
    [IsDefault]         BIT CONSTRAINT [DF_cmsDocumentType_IsDefault] DEFAULT ('0') NOT NULL,
    CONSTRAINT [PK_cmsDocumentType] PRIMARY KEY CLUSTERED ([contentTypeNodeId] ASC, [templateNodeId] ASC),
    CONSTRAINT [FK_cmsDocumentType_cmsContentType_nodeId] FOREIGN KEY ([contentTypeNodeId]) REFERENCES [dbo].[cmsContentType] ([nodeId]),
    CONSTRAINT [FK_cmsDocumentType_cmsTemplate_nodeId] FOREIGN KEY ([templateNodeId]) REFERENCES [dbo].[cmsTemplate] ([nodeId]),
    CONSTRAINT [FK_cmsDocumentType_umbracoNode_id] FOREIGN KEY ([contentTypeNodeId]) REFERENCES [dbo].[umbracoNode] ([id])
);

