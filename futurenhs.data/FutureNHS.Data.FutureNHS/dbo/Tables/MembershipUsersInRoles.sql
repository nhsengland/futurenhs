CREATE TABLE [dbo].[MembershipUsersInRoles] (
    [UserIdentifier] UNIQUEIDENTIFIER NOT NULL,
    [RoleIdentifier] UNIQUEIDENTIFIER NOT NULL,
    [RowVersion] ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.MembershipUsersInRoles] PRIMARY KEY CLUSTERED ([UserIdentifier] ASC, [RoleIdentifier] ASC),
    CONSTRAINT [FK_dbo.MembershipUsersInRoles_dbo.MembershipRole_RoleIdentifier] FOREIGN KEY ([RoleIdentifier]) REFERENCES [dbo].[MembershipRole] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.MembershipUsersInRoles_dbo.MembershipUser_UserIdentifier] FOREIGN KEY ([UserIdentifier]) REFERENCES [dbo].[MembershipUser] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_RoleIdentifier]
    ON [dbo].[MembershipUsersInRoles]([RoleIdentifier] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserIdentifier]
    ON [dbo].[MembershipUsersInRoles]([UserIdentifier] ASC);

