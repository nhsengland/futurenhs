CREATE TABLE [dbo].[GroupInvite] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [EmailAddress] NVARCHAR (254)   NOT NULL,
    [GroupId]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedAtUTC] DATETIME2         NOT NULL,
    [ExpiresAtUTC] DATETIME2         NULL,
    [IsDeleted]    BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]   ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.GroupInvite] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.GroupInvite_dbo.Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Group] ([Id])
);

