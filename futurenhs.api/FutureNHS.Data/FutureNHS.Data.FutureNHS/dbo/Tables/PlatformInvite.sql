CREATE TABLE [dbo].[PlatformInvite] (
    [Id]           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [EmailAddress] NVARCHAR (254)   NOT NULL,
    [GroupId]      UNIQUEIDENTIFIER NULL,
    [CreatedAtUTC] DATETIME2         NOT NULL,
    [CreatedBy]    UNIQUEIDENTIFIER  NULL,
    [ExpiresAtUTC] DATETIME2         NULL,
    [IsDeleted]    BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]   ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.PlatformInvite] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [PK_dbo.PlatformInvite_EmailAddress_GroupId] UNIQUE NONCLUSTERED ([EmailAddress] ASC, [GroupId] ASC),
    CONSTRAINT [FK_dbo.PlatformInvite_dbo.Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Group] ([Id]),
    CONSTRAINT [FK_dbo.PlatformInvite_dbo.MembershipUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
);

