CREATE TABLE [dbo].[cmsMember] (
    [nodeId]             INT             NOT NULL,
    [Email]              NVARCHAR (1000) CONSTRAINT [DF_cmsMember_Email] DEFAULT ('''') NOT NULL,
    [LoginName]          NVARCHAR (1000) CONSTRAINT [DF_cmsMember_LoginName] DEFAULT ('''') NOT NULL,
    [Password]           NVARCHAR (1000) CONSTRAINT [DF_cmsMember_Password] DEFAULT ('''') NOT NULL,
    [passwordConfig]     NVARCHAR (500)  NULL,
    [securityStampToken] NVARCHAR (255)  NULL,
    [emailConfirmedDate] DATETIME        NULL,
    CONSTRAINT [PK_cmsMember] PRIMARY KEY CLUSTERED ([nodeId] ASC),
    CONSTRAINT [FK_cmsMember_umbracoContent_nodeId] FOREIGN KEY ([nodeId]) REFERENCES [dbo].[umbracoContent] ([nodeId])
);


GO
CREATE NONCLUSTERED INDEX [IX_cmsMember_LoginName]
    ON [dbo].[cmsMember]([LoginName] ASC);

