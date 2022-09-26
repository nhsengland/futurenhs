CREATE TABLE [dbo].[umbracoUserGroup2NodePermission] (
    [userGroupId] INT            NOT NULL,
    [nodeId]      INT            NOT NULL,
    [permission]  NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_umbracoUserGroup2NodePermission] PRIMARY KEY CLUSTERED ([userGroupId] ASC, [nodeId] ASC, [permission] ASC),
    CONSTRAINT [FK_umbracoUserGroup2NodePermission_umbracoNode_id] FOREIGN KEY ([nodeId]) REFERENCES [dbo].[umbracoNode] ([id]),
    CONSTRAINT [FK_umbracoUserGroup2NodePermission_umbracoUserGroup_id] FOREIGN KEY ([userGroupId]) REFERENCES [dbo].[umbracoUserGroup] ([id])
);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoUser2NodePermission_nodeId]
    ON [dbo].[umbracoUserGroup2NodePermission]([nodeId] ASC);

