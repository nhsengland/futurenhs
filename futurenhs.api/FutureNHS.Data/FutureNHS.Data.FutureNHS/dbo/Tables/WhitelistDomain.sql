CREATE TABLE [dbo].[WhitelistDomain] (
    [Id]        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [EmailDomain]     NVARCHAR (200)   NOT NULL,
    [IsDeleted] BIT  DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.WhitelistDomain] PRIMARY KEY CLUSTERED ([Id] ASC)
    );