CREATE TABLE [dbo].[umbracoExternalLogin] (
    [id]            INT             IDENTITY (1, 1) NOT NULL,
    [userId]        INT             NOT NULL,
    [loginProvider] NVARCHAR (400)  NOT NULL,
    [providerKey]   NVARCHAR (4000) NOT NULL,
    [createDate]    DATETIME        CONSTRAINT [DF_umbracoExternalLogin_createDate] DEFAULT (getdate()) NOT NULL,
    [userData]      NTEXT           NULL,
    CONSTRAINT [PK_umbracoExternalLogin] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoExternalLogin_userId]
    ON [dbo].[umbracoExternalLogin]([userId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoExternalLogin_LoginProvider]
    ON [dbo].[umbracoExternalLogin]([loginProvider] ASC, [userId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoExternalLogin_ProviderKey]
    ON [dbo].[umbracoExternalLogin]([loginProvider] ASC, [providerKey] ASC);

