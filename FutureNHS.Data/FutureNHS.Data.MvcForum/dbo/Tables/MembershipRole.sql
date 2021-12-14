CREATE TABLE [dbo].[MembershipRole] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [RoleName] NVARCHAR (256)   NOT NULL,
    CONSTRAINT [PK_dbo.MembershipRole] PRIMARY KEY CLUSTERED ([Id] ASC)
);

