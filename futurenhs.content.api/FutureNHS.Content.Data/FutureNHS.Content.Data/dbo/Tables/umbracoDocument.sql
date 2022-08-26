CREATE TABLE [dbo].[umbracoDocument] (
    [nodeId]    INT NOT NULL,
    [published] BIT NOT NULL,
    [edited]    BIT NOT NULL,
    CONSTRAINT [PK_umbracoDocument] PRIMARY KEY CLUSTERED ([nodeId] ASC),
    CONSTRAINT [FK_umbracoDocument_umbracoContent_nodeId] FOREIGN KEY ([nodeId]) REFERENCES [dbo].[umbracoContent] ([nodeId])
);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoDocument_Published]
    ON [dbo].[umbracoDocument]([published] ASC);

