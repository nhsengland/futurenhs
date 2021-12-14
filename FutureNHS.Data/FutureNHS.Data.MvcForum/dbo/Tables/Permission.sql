CREATE TABLE [dbo].[Permission] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Name]     NVARCHAR (150)   NOT NULL,
    [IsGlobal] BIT              NOT NULL,
    CONSTRAINT [PK_dbo.Permission] PRIMARY KEY CLUSTERED ([Id] ASC)
);

