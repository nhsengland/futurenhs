CREATE TABLE [dbo].[umbracoUser2NodeNotify] (
    [userId] INT       NOT NULL,
    [nodeId] INT       NOT NULL,
    [action] NCHAR (1) NOT NULL,
    CONSTRAINT [PK_umbracoUser2NodeNotify] PRIMARY KEY CLUSTERED ([userId] ASC, [nodeId] ASC, [action] ASC),
    CONSTRAINT [FK_umbracoUser2NodeNotify_umbracoNode_id] FOREIGN KEY ([nodeId]) REFERENCES [dbo].[umbracoNode] ([id]),
    CONSTRAINT [FK_umbracoUser2NodeNotify_umbracoUser_id] FOREIGN KEY ([userId]) REFERENCES [dbo].[umbracoUser] ([id])
);

