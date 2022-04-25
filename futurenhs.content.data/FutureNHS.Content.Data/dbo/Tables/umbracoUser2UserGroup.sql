CREATE TABLE [dbo].[umbracoUser2UserGroup] (
    [userId]      INT NOT NULL,
    [userGroupId] INT NOT NULL,
    CONSTRAINT [PK_user2userGroup] PRIMARY KEY CLUSTERED ([userId] ASC, [userGroupId] ASC),
    CONSTRAINT [FK_umbracoUser2UserGroup_umbracoUser_id] FOREIGN KEY ([userId]) REFERENCES [dbo].[umbracoUser] ([id]),
    CONSTRAINT [FK_umbracoUser2UserGroup_umbracoUserGroup_id] FOREIGN KEY ([userGroupId]) REFERENCES [dbo].[umbracoUserGroup] ([id])
);

