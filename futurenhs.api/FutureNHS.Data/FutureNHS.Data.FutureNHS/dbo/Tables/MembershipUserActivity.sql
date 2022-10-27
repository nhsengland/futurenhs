CREATE TABLE [dbo].[MembershipUserActivity] (
    [MembershipUserId]               UNIQUEIDENTIFIER NOT NULL,
    [LastActivityDateUTC]            DATETIME2        NULL
    CONSTRAINT [PK_dbo.MembershipUserActivity] PRIMARY KEY CLUSTERED ([MembershipUserId] ASC),
    CONSTRAINT [FK_dbo.MembershipUserActivity_dbo.MembershipUser] FOREIGN KEY ([MembershipUserId]) REFERENCES [dbo].[MembershipUser] ([Id]),
);
