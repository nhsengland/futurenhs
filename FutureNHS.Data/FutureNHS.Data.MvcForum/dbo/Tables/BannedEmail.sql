CREATE TABLE [dbo].[BannedEmail] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [Email]     NVARCHAR (200)   NOT NULL,
    [DateAdded] DATETIME         NOT NULL,
    CONSTRAINT [PK_dbo.BannedEmail] PRIMARY KEY CLUSTERED ([Id] ASC)
);

