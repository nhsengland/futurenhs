CREATE TABLE [dbo].[umbracoAccess] (
    [id]             UNIQUEIDENTIFIER NOT NULL,
    [nodeId]         INT              NOT NULL,
    [loginNodeId]    INT              NOT NULL,
    [noAccessNodeId] INT              NOT NULL,
    [createDate]     DATETIME         CONSTRAINT [DF_umbracoAccess_createDate] DEFAULT (getdate()) NOT NULL,
    [updateDate]     DATETIME         CONSTRAINT [DF_umbracoAccess_updateDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_umbracoAccess] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoAccess_umbracoNode_id] FOREIGN KEY ([nodeId]) REFERENCES [dbo].[umbracoNode] ([id]),
    CONSTRAINT [FK_umbracoAccess_umbracoNode_id1] FOREIGN KEY ([loginNodeId]) REFERENCES [dbo].[umbracoNode] ([id]),
    CONSTRAINT [FK_umbracoAccess_umbracoNode_id2] FOREIGN KEY ([noAccessNodeId]) REFERENCES [dbo].[umbracoNode] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoAccess_nodeId]
    ON [dbo].[umbracoAccess]([nodeId] ASC);

