CREATE TABLE [dbo].[umbracoLogViewerQuery] (
    [id]    INT            IDENTITY (1, 1) NOT NULL,
    [name]  NVARCHAR (255) NOT NULL,
    [query] NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_umbracoLogViewerQuery] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_LogViewerQuery_name]
    ON [dbo].[umbracoLogViewerQuery]([name] ASC);

