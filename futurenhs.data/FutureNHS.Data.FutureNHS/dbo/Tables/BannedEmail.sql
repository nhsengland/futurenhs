CREATE TABLE [dbo].[BannedEmail] (
    [Id]        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Email]     NVARCHAR (200)   NOT NULL,
    [CreatedAtUTC] DATETIME         NOT NULL,
    [IsDeleted] BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion] ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.BannedEmail] PRIMARY KEY CLUSTERED ([Id] ASC)
);

