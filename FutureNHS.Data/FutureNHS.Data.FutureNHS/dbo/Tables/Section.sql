CREATE TABLE [dbo].[Section] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Name]               NVARCHAR (450)   NOT NULL,
    [Description]        NVARCHAR (4000)   NULL,
    [SortOrder]          INT              NOT NULL,
    [CreatedAtUTC]        DATETIME2         NOT NULL,
    [IsDeleted] BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion] ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.Section] PRIMARY KEY CLUSTERED ([Id] ASC)
);

