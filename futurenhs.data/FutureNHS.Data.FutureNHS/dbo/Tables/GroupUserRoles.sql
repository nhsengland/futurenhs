CREATE TABLE [dbo].[GroupUserRoles]
(
	[Id]       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [RoleName] NVARCHAR (256)   NOT NULL,
    [IsDeleted] BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion] ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.GroupUserRole] PRIMARY KEY CLUSTERED ([Id] ASC)
);
