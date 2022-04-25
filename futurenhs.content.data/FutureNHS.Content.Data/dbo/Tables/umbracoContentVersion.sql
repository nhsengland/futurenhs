CREATE TABLE [dbo].[umbracoContentVersion] (
    [id]             INT            IDENTITY (1, 1) NOT NULL,
    [nodeId]         INT            NOT NULL,
    [versionDate]    DATETIME       CONSTRAINT [DF_umbracoContentVersion_versionDate] DEFAULT (getdate()) NOT NULL,
    [userId]         INT            NULL,
    [current]        BIT            NOT NULL,
    [text]           NVARCHAR (255) NULL,
    [preventCleanup] BIT            CONSTRAINT [DF_umbracoContentVersion_preventCleanup] DEFAULT ('0') NOT NULL,
    CONSTRAINT [PK_umbracoContentVersion] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoContentVersion_umbracoContent_nodeId] FOREIGN KEY ([nodeId]) REFERENCES [dbo].[umbracoContent] ([nodeId]),
    CONSTRAINT [FK_umbracoContentVersion_umbracoUser_id] FOREIGN KEY ([userId]) REFERENCES [dbo].[umbracoUser] ([id])
);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoContentVersion_NodeId]
    ON [dbo].[umbracoContentVersion]([nodeId] ASC, [current] ASC)
    INCLUDE([id], [versionDate], [text], [userId]);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoContentVersion_Current]
    ON [dbo].[umbracoContentVersion]([current] ASC)
    INCLUDE([nodeId]);

