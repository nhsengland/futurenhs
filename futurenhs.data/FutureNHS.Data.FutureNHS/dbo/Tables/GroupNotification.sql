CREATE TABLE [dbo].[GroupNotification] (
    [Id]                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Group_Id]          UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id] UNIQUEIDENTIFIER NOT NULL,
    [IsDeleted]         BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]        ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.GroupNotification] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.GroupNotification_dbo.Group_Group_Id] FOREIGN KEY ([Group_Id]) REFERENCES [dbo].[Group] ([Id]),
    CONSTRAINT [FK_dbo.GroupNotification_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Group_Id]
    ON [dbo].[GroupNotification]([Group_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[GroupNotification]([MembershipUser_Id] ASC);

