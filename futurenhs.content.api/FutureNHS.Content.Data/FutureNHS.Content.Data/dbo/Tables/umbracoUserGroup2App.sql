CREATE TABLE [dbo].[umbracoUserGroup2App] (
    [userGroupId] INT           NOT NULL,
    [app]         NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_userGroup2App] PRIMARY KEY CLUSTERED ([userGroupId] ASC, [app] ASC),
    CONSTRAINT [FK_umbracoUserGroup2App_umbracoUserGroup_id] FOREIGN KEY ([userGroupId]) REFERENCES [dbo].[umbracoUserGroup] ([id])
);

