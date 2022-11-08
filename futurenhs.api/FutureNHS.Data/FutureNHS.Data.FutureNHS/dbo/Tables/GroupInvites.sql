CREATE TABLE [dbo].[GroupInvites] (
    [Id]           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [MembershipUser_Id]     UNIQUEIDENTIFIER NOT NULL,
    [GroupId]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedAtUTC] DATETIME2         NOT NULL,
    [CreatedBy]    UNIQUEIDENTIFIER  NULL,
    [ExpiresAtUTC] DATETIME2         NULL,
    [IsDeleted]    BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]   ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.GroupInvites] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.GroupInvites_dbo.Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Group] ([Id]),
    CONSTRAINT [FK_dbo.GroupInvites_dbo.MembershipUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.GroupInvites_dbo.MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id])
);

