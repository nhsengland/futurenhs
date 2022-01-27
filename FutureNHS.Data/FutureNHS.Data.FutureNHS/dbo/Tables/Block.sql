CREATE TABLE [dbo].[Block] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Date]       DATETIME         NOT NULL,
    [Blocked_Id] UNIQUEIDENTIFIER NOT NULL,
    [Blocker_Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.Block] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Block_dbo.MembershipUser_Blocked_Id] FOREIGN KEY ([Blocked_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Block_dbo.MembershipUser_Blocker_Id] FOREIGN KEY ([Blocker_Id]) REFERENCES [dbo].[MembershipUser] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Blocked_Id]
    ON [dbo].[Block]([Blocked_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Blocker_Id]
    ON [dbo].[Block]([Blocker_Id] ASC);

