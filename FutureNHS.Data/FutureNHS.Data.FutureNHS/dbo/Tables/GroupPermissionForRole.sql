CREATE TABLE [dbo].[GroupPermissionForRole] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [IsTicked]          BIT              NOT NULL,
    [Group_Id]          UNIQUEIDENTIFIER NOT NULL,
    [MembershipRole_Id] UNIQUEIDENTIFIER NOT NULL,
    [Permission_Id]     UNIQUEIDENTIFIER NOT NULL,
    [IsDeleted]         BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]        ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.GroupPermissionForRole] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.GroupPermissionForRole_dbo.Group_Group_Id] FOREIGN KEY ([Group_Id]) REFERENCES [dbo].[Group] ([Id]),
    CONSTRAINT [FK_dbo.GroupPermissionForRole_dbo.MembershipRole_MembershipRole_Id] FOREIGN KEY ([MembershipRole_Id]) REFERENCES [dbo].[MembershipRole] ([Id]),
    CONSTRAINT [FK_dbo.GroupPermissionForRole_dbo.Permission_Permission_Id] FOREIGN KEY ([Permission_Id]) REFERENCES [dbo].[Permission] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Group_Id]
    ON [dbo].[GroupPermissionForRole]([Group_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipRole_Id]
    ON [dbo].[GroupPermissionForRole]([MembershipRole_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Permission_Id]
    ON [dbo].[GroupPermissionForRole]([Permission_Id] ASC);

