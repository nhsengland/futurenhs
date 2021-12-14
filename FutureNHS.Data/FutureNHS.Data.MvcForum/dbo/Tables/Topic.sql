CREATE TABLE [dbo].[Topic] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (450)   NOT NULL,
    [CreateDate]         DATETIME         NOT NULL,
    [Solved]             BIT              NOT NULL,
    [SolvedReminderSent] BIT              NULL,
    [Slug]               NVARCHAR (450)   NOT NULL,
    [Views]              INT              NULL,
    [IsSticky]           BIT              NOT NULL,
    [IsLocked]           BIT              NOT NULL,
    [Pending]            BIT              NULL,
    [ExtendedDataString] NVARCHAR (MAX)   NULL,
    [Group_Id]           UNIQUEIDENTIFIER NOT NULL,
    [Post_Id]            UNIQUEIDENTIFIER NULL,
    [Poll_Id]            UNIQUEIDENTIFIER NULL,
    [MembershipUser_Id]  UNIQUEIDENTIFIER NOT NULL,
    [Category_Id]        UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_dbo.Topic] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Topic_dbo.Category_Category_Id] FOREIGN KEY ([Category_Id]) REFERENCES [dbo].[Category] ([Id]),
    CONSTRAINT [FK_dbo.Topic_dbo.Group_Group_Id] FOREIGN KEY ([Group_Id]) REFERENCES [dbo].[Group] ([Id]),
    CONSTRAINT [FK_dbo.Topic_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Topic_dbo.Poll_Poll_Id] FOREIGN KEY ([Poll_Id]) REFERENCES [dbo].[Poll] ([Id]),
    CONSTRAINT [FK_dbo.Topic_dbo.Post_Post_Id] FOREIGN KEY ([Post_Id]) REFERENCES [dbo].[Post] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Category_Id]
    ON [dbo].[Topic]([Category_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Group_Id]
    ON [dbo].[Topic]([Group_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Topic]([MembershipUser_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Poll_Id]
    ON [dbo].[Topic]([Poll_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Post_Id]
    ON [dbo].[Topic]([Post_Id] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Topic_Slug]
    ON [dbo].[Topic]([Slug] ASC);

