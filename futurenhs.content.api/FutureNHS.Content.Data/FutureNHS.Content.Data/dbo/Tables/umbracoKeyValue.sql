CREATE TABLE [dbo].[umbracoKeyValue] (
    [key]     NVARCHAR (256) NOT NULL,
    [value]   NVARCHAR (255) NULL,
    [updated] DATETIME       CONSTRAINT [DF_umbracoKeyValue_updated] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_umbracoKeyValue] PRIMARY KEY CLUSTERED ([key] ASC)
);

