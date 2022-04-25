CREATE TABLE [dbo].[umbracoLock] (
    [id]    INT           NOT NULL,
    [value] INT           NOT NULL,
    [name]  NVARCHAR (64) NOT NULL,
    CONSTRAINT [PK_umbracoLock] PRIMARY KEY CLUSTERED ([id] ASC)
);

