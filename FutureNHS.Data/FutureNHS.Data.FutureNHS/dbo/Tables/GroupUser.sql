CREATE TABLE [dbo].[GroupUser] (
    [Id]                         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Approved]                   BIT              NOT NULL,
    [Rejected]                   BIT              NOT NULL,
    [Locked]                     BIT              NOT NULL,
    [Banned]                     BIT              NOT NULL,
    [RequestToJoinDateUTC]          DATETIME         NOT NULL,
    [ApprovedToJoinDateUTC]         DATETIME         NULL,
    [RequestToJoinReason]        NVARCHAR (200)   NULL,
    [LockReason]                 NVARCHAR (200)   NULL,
    [BanReason]                  NVARCHAR (200)   NULL,
    [ApprovingMembershipUser_Id] UNIQUEIDENTIFIER NULL,
    [MembershipRole_Id]          UNIQUEIDENTIFIER NULL,
    [MembershipUser_Id]          UNIQUEIDENTIFIER NOT NULL,
    [Group_Id]                   UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]                 ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.GroupUser_MembershipUser_Id_Group_Id] PRIMARY KEY CLUSTERED ([MembershipUser_Id] ASC, [Group_Id] ASC),
    CONSTRAINT [FK_dbo.GroupUser_dbo.Group_Group_Id] FOREIGN KEY ([Group_Id]) REFERENCES [dbo].[Group] ([Id]),
    CONSTRAINT [FK_dbo.GroupUser_dbo.MembershipRole_MembershipRole_Id] FOREIGN KEY ([MembershipRole_Id]) REFERENCES [dbo].[MembershipRole] ([Id]),
    CONSTRAINT [FK_dbo.GroupUser_dbo.MembershipUser_ApprovingMembershipUser_Id] FOREIGN KEY ([ApprovingMembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.GroupUser_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ApprovingMembershipUser_Id]
    ON [dbo].[GroupUser]([ApprovingMembershipUser_Id] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Group_Id]
    ON [dbo].[GroupUser]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipRole_Id]
    ON [dbo].[GroupUser]([MembershipRole_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[GroupUser]([MembershipUser_Id] ASC);

