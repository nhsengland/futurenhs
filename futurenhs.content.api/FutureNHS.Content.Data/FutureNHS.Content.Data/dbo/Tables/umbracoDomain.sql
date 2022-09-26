CREATE TABLE [dbo].[umbracoDomain] (
    [id]                    INT            IDENTITY (1, 1) NOT NULL,
    [domainDefaultLanguage] INT            NULL,
    [domainRootStructureID] INT            NULL,
    [domainName]            NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_umbracoDomain] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoDomain_umbracoNode_id] FOREIGN KEY ([domainRootStructureID]) REFERENCES [dbo].[umbracoNode] ([id])
);

