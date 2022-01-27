CREATE TABLE [dbo].[GroupInvite] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [EmailAddress] NVARCHAR (254)   NOT NULL,
    [IsDeleted]    BIT              NOT NULL,
    [GroupId]      UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.GroupInvite] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.GroupInvite_dbo.Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Group] ([Id])
);

