CREATE TABLE [dbo].[umbracoRedirectUrl] (
    [id]            UNIQUEIDENTIFIER NOT NULL,
    [contentKey]    UNIQUEIDENTIFIER NOT NULL,
    [createDateUtc] DATETIME         NOT NULL,
    [url]           NVARCHAR (255)   NOT NULL,
    [culture]       NVARCHAR (255)   NULL,
    [urlHash]       NVARCHAR (40)    NOT NULL,
    CONSTRAINT [PK_umbracoRedirectUrl] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoRedirectUrl_umbracoNode_uniqueID] FOREIGN KEY ([contentKey]) REFERENCES [dbo].[umbracoNode] ([uniqueId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoRedirectUrl]
    ON [dbo].[umbracoRedirectUrl]([urlHash] ASC, [contentKey] ASC, [culture] ASC, [createDateUtc] ASC);

