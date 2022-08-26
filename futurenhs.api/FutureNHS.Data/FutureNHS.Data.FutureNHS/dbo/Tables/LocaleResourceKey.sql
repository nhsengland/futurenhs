CREATE TABLE [dbo].[LocaleResourceKey] (
    [Id]        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Name]      NVARCHAR (200)   NOT NULL,
    [Notes]     NVARCHAR (MAX)   NULL,
    [DateAdded] DATETIME         NOT NULL,
    CONSTRAINT [PK_dbo.LocaleResourceKey] PRIMARY KEY CLUSTERED ([Id] ASC)
);

