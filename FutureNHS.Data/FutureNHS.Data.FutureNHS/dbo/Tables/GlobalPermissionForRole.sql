CREATE TABLE [dbo].[GlobalPermissionForRole] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [IsTicked]          BIT              NOT NULL,
    [MembershipRole_Id] UNIQUEIDENTIFIER NOT NULL,
    [Permission_Id]     UNIQUEIDENTIFIER NOT NULL,
    [IsDeleted]         BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]        ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.GlobalPermissionForRole] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.GlobalPermissionForRole_dbo.MembershipRole_MembershipRole_Id] FOREIGN KEY ([MembershipRole_Id]) REFERENCES [dbo].[MembershipRole] ([Id]),
    CONSTRAINT [FK_dbo.GlobalPermissionForRole_dbo.Permission_Permission_Id] FOREIGN KEY ([Permission_Id]) REFERENCES [dbo].[Permission] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipRole_Id]
    ON [dbo].[GlobalPermissionForRole]([MembershipRole_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Permission_Id]
    ON [dbo].[GlobalPermissionForRole]([Permission_Id] ASC);

