CREATE TABLE [dbo].[DiscussionNotification] (
    [Id]                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Discussion_Id]          UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id] UNIQUEIDENTIFIER NOT NULL,
    [RowVersion]         ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.DiscussionNotification] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.DiscussionNotification_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.DiscussionNotification_dbo.Discussion_Discussion_Id] FOREIGN KEY ([Discussion_Id]) REFERENCES [dbo].[Discussion] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[DiscussionNotification]([MembershipUser_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Discussion_Id]
    ON [dbo].[DiscussionNotification]([Discussion_Id] ASC);

