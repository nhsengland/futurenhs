CREATE TABLE [dbo].[MembershipRole] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [RoleName] NVARCHAR (256)   NOT NULL,
    [IsDeleted] BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion] ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.MembershipRole] PRIMARY KEY CLUSTERED ([Id] ASC)
);

