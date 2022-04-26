CREATE TABLE [dbo].[umbracoServer] (
    [id]                    INT            IDENTITY (1, 1) NOT NULL,
    [address]               NVARCHAR (500) NOT NULL,
    [computerName]          NVARCHAR (255) NOT NULL,
    [registeredDate]        DATETIME       CONSTRAINT [DF_umbracoServer_registeredDate] DEFAULT (getdate()) NOT NULL,
    [lastNotifiedDate]      DATETIME       NOT NULL,
    [isActive]              BIT            NOT NULL,
    [isSchedulingPublisher] BIT            NOT NULL,
    CONSTRAINT [PK_umbracoServer] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_computerName]
    ON [dbo].[umbracoServer]([computerName] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoServer_isActive]
    ON [dbo].[umbracoServer]([isActive] ASC);

