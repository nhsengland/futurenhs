CREATE TABLE [dbo].[Section] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (450)   NOT NULL,
    [Description]        NVARCHAR (MAX)   NULL,
    [SortOrder]          INT              NOT NULL,
    [DateCreated]        DATETIME         NOT NULL,
    [ExtendedDataString] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_dbo.Section] PRIMARY KEY CLUSTERED ([Id] ASC)
);

