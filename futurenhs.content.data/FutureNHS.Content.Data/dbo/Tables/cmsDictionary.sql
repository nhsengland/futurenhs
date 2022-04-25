CREATE TABLE [dbo].[cmsDictionary] (
    [pk]     INT              IDENTITY (1, 1) NOT NULL,
    [id]     UNIQUEIDENTIFIER NOT NULL,
    [parent] UNIQUEIDENTIFIER NULL,
    [key]    NVARCHAR (450)   NOT NULL,
    CONSTRAINT [PK_cmsDictionary] PRIMARY KEY CLUSTERED ([pk] ASC),
    CONSTRAINT [FK_cmsDictionary_cmsDictionary_id] FOREIGN KEY ([parent]) REFERENCES [dbo].[cmsDictionary] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_cmsDictionary_id]
    ON [dbo].[cmsDictionary]([id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_cmsDictionary_Parent]
    ON [dbo].[cmsDictionary]([parent] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_cmsDictionary_key]
    ON [dbo].[cmsDictionary]([key] ASC);

