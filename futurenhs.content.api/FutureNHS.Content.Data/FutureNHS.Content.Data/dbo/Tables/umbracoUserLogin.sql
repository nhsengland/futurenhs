CREATE TABLE [dbo].[umbracoUserLogin] (
    [sessionId]        UNIQUEIDENTIFIER NOT NULL,
    [userId]           INT              NOT NULL,
    [loggedInUtc]      DATETIME         NOT NULL,
    [lastValidatedUtc] DATETIME         NOT NULL,
    [loggedOutUtc]     DATETIME         NULL,
    [ipAddress]        NVARCHAR (255)   NULL,
    CONSTRAINT [PK_umbracoUserLogin] PRIMARY KEY CLUSTERED ([sessionId] ASC),
    CONSTRAINT [FK_umbracoUserLogin_umbracoUser_id] FOREIGN KEY ([userId]) REFERENCES [dbo].[umbracoUser] ([id])
);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoUserLogin_lastValidatedUtc]
    ON [dbo].[umbracoUserLogin]([lastValidatedUtc] ASC);

