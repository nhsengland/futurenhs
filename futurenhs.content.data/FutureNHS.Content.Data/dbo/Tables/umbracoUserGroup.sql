CREATE TABLE [dbo].[umbracoUserGroup] (
    [id]                          INT            IDENTITY (1, 1) NOT NULL,
    [userGroupAlias]              NVARCHAR (200) NOT NULL,
    [userGroupName]               NVARCHAR (200) NOT NULL,
    [userGroupDefaultPermissions] NVARCHAR (50)  NULL,
    [createDate]                  DATETIME       CONSTRAINT [DF_umbracoUserGroup_createDate] DEFAULT (getdate()) NOT NULL,
    [updateDate]                  DATETIME       CONSTRAINT [DF_umbracoUserGroup_updateDate] DEFAULT (getdate()) NOT NULL,
    [icon]                        NVARCHAR (255) NULL,
    [startContentId]              INT            NULL,
    [startMediaId]                INT            NULL,
    CONSTRAINT [PK_umbracoUserGroup] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_startContentId_umbracoNode_id] FOREIGN KEY ([startContentId]) REFERENCES [dbo].[umbracoNode] ([id]),
    CONSTRAINT [FK_startMediaId_umbracoNode_id] FOREIGN KEY ([startMediaId]) REFERENCES [dbo].[umbracoNode] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoUserGroup_userGroupAlias]
    ON [dbo].[umbracoUserGroup]([userGroupAlias] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoUserGroup_userGroupName]
    ON [dbo].[umbracoUserGroup]([userGroupName] ASC);

