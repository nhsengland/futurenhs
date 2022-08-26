CREATE TABLE [dbo].[umbracoUserGroup2Node] (
    [userGroupId] INT NOT NULL,
    [nodeId]      INT NOT NULL,
    CONSTRAINT [PK_umbracoUserGroup2Node] PRIMARY KEY CLUSTERED ([userGroupId] ASC, [nodeId] ASC),
    CONSTRAINT [FK_umbracoUserGroup2Node_umbracoNode_id] FOREIGN KEY ([nodeId]) REFERENCES [dbo].[umbracoNode] ([id]),
    CONSTRAINT [FK_umbracoUserGroup2Node_umbracoUserGroup_id] FOREIGN KEY ([userGroupId]) REFERENCES [dbo].[umbracoUserGroup] ([id])
);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoUserGroup2Node_nodeId]
    ON [dbo].[umbracoUserGroup2Node]([nodeId] ASC);

