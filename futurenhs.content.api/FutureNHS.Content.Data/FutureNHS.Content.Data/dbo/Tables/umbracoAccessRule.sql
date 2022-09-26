CREATE TABLE [dbo].[umbracoAccessRule] (
    [id]         UNIQUEIDENTIFIER NOT NULL,
    [accessId]   UNIQUEIDENTIFIER NOT NULL,
    [ruleValue]  NVARCHAR (255)   NOT NULL,
    [ruleType]   NVARCHAR (255)   NOT NULL,
    [createDate] DATETIME         CONSTRAINT [DF_umbracoAccessRule_createDate] DEFAULT (getdate()) NOT NULL,
    [updateDate] DATETIME         CONSTRAINT [DF_umbracoAccessRule_updateDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_umbracoAccessRule] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoAccessRule_umbracoAccess_id] FOREIGN KEY ([accessId]) REFERENCES [dbo].[umbracoAccess] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoAccessRule]
    ON [dbo].[umbracoAccessRule]([ruleValue] ASC, [ruleType] ASC, [accessId] ASC);

