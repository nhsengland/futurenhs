CREATE TABLE [dbo].[Language] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [Name]              NVARCHAR (100)   NOT NULL,
    [LanguageCulture]   NVARCHAR (20)    NOT NULL,
    [FlagImageFileName] NVARCHAR (50)    NULL,
    [RightToLeft]       BIT              NOT NULL,
    CONSTRAINT [PK_dbo.Language] PRIMARY KEY CLUSTERED ([Id] ASC)
);

