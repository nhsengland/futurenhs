CREATE TABLE [dbo].[umbracoMediaVersion] (
    [id]   INT            NOT NULL,
    [path] NVARCHAR (255) NULL,
    CONSTRAINT [PK_umbracoMediaVersion] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoMediaVersion_umbracoContentVersion_id] FOREIGN KEY ([id]) REFERENCES [dbo].[umbracoContentVersion] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoMediaVersion]
    ON [dbo].[umbracoMediaVersion]([id] ASC, [path] ASC);

