CREATE TABLE [dbo].[LocaleStringResource] (
    [Id]                   UNIQUEIDENTIFIER NOT NULL,
    [ResourceValue]        NVARCHAR (1000)  NOT NULL,
    [LocaleResourceKey_Id] UNIQUEIDENTIFIER NOT NULL,
    [Language_Id]          UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.LocaleStringResource] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.LocaleStringResource_dbo.Language_Language_Id] FOREIGN KEY ([Language_Id]) REFERENCES [dbo].[Language] ([Id]),
    CONSTRAINT [FK_dbo.LocaleStringResource_dbo.LocaleResourceKey_LocaleResourceKey_Id] FOREIGN KEY ([LocaleResourceKey_Id]) REFERENCES [dbo].[LocaleResourceKey] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Language_Id]
    ON [dbo].[LocaleStringResource]([Language_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LocaleResourceKey_Id]
    ON [dbo].[LocaleStringResource]([LocaleResourceKey_Id] ASC);

