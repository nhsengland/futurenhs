CREATE TABLE [dbo].[umbracoExternalLoginToken] (
    [id]              INT            IDENTITY (1, 1) NOT NULL,
    [externalLoginId] INT            NOT NULL,
    [name]            NVARCHAR (255) NOT NULL,
    [value]           NVARCHAR (MAX) NOT NULL,
    [createDate]      DATETIME       CONSTRAINT [DF_umbracoExternalLoginToken_createDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_umbracoExternalLoginToken] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoExternalLoginToken_umbracoExternalLogin_id] FOREIGN KEY ([externalLoginId]) REFERENCES [dbo].[umbracoExternalLogin] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoExternalLoginToken_Name]
    ON [dbo].[umbracoExternalLoginToken]([externalLoginId] ASC, [name] ASC);

