CREATE TABLE [dbo].[GroupUsersInRoles]
(
	[GroupUser_Id] UNIQUEIDENTIFIER NOT NULL, 
    [GroupRole_Id] UNIQUEIDENTIFIER NOT NULL

    CONSTRAINT [PK_dbo.GroupUsersInRoles_Id] PRIMARY KEY CLUSTERED ([GroupUser_Id], [GroupRole_Id] ASC),
    CONSTRAINT [FK_dbo.GroupUsersInRoles_dbo.GroupUser_Id_GroupUser_Id] FOREIGN KEY ([GroupUser_Id]) REFERENCES [dbo].[GroupUser]([Id]),
    CONSTRAINT [FK_dbo.GroupUsersInRoles_dbo.GroupRole_Id_GroupRole_Id] FOREIGN KEY ([GroupRole_Id]) REFERENCES [dbo].[GroupUserRoles]([Id])
)
