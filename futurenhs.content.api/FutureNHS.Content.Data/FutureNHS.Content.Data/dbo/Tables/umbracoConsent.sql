CREATE TABLE [dbo].[umbracoConsent] (
    [id]         INT            IDENTITY (1, 1) NOT NULL,
    [current]    BIT            NOT NULL,
    [source]     NVARCHAR (512) NOT NULL,
    [context]    NVARCHAR (128) NOT NULL,
    [action]     NVARCHAR (512) NOT NULL,
    [createDate] DATETIME       CONSTRAINT [DF_umbracoConsent_createDate] DEFAULT (getdate()) NOT NULL,
    [state]      INT            NOT NULL,
    [comment]    NVARCHAR (255) NULL,
    CONSTRAINT [PK_umbracoConsent] PRIMARY KEY CLUSTERED ([id] ASC)
);

