CREATE TABLE [dbo].[BannedWord] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Word]       NVARCHAR (75)    NOT NULL,
    [IsStopWord] BIT              NULL,
    [DateAdded]  DATETIME         NOT NULL,
    CONSTRAINT [PK_dbo.BannedWord] PRIMARY KEY CLUSTERED ([Id] ASC)
);

