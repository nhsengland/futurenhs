CREATE TABLE [dbo].[umbracoLog] (
    [id]         INT             IDENTITY (1, 1) NOT NULL,
    [userId]     INT             NULL,
    [NodeId]     INT             NOT NULL,
    [entityType] NVARCHAR (50)   NULL,
    [Datestamp]  DATETIME        CONSTRAINT [DF_umbracoLog_Datestamp] DEFAULT (getdate()) NOT NULL,
    [logHeader]  NVARCHAR (50)   NOT NULL,
    [logComment] NVARCHAR (4000) NULL,
    [parameters] NVARCHAR (500)  NULL,
    CONSTRAINT [PK_umbracoLog] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoLog_umbracoUser_id] FOREIGN KEY ([userId]) REFERENCES [dbo].[umbracoUser] ([id])
);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoLog]
    ON [dbo].[umbracoLog]([NodeId] ASC);

