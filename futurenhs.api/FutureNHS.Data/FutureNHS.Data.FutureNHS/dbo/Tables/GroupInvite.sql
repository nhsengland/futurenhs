CREATE TABLE [dbo].[GroupInvite] (
    [Id]           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [EmailAddress] NVARCHAR (254)   NOT NULL UNIQUE,
    [GroupId]      UNIQUEIDENTIFIER NULL,
    [CreatedAtUTC] DATETIME2         NOT NULL,
    [CreatedBy]    UNIQUEIDENTIFIER  NULL,
    [ExpiresAtUTC] DATETIME2         NULL,
    [IsDeleted]    BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]   ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.GroupInvite] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.GroupInvite_dbo.Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Group] ([Id]),
    CONSTRAINT [FK_dbo.GroupInvite_dbo.MembershipUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
);

