CREATE TABLE [dbo].[BannedWord] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Word]       NVARCHAR (75)    NOT NULL,
    [IsStopWord] BIT              NULL,
    [CreatedAtUTC]  DATETIME         NOT NULL,
    CONSTRAINT [PK_dbo.BannedWord] PRIMARY KEY CLUSTERED ([Id] ASC)
);

