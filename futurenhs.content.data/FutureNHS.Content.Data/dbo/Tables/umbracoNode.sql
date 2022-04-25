CREATE TABLE [dbo].[umbracoNode] (
    [id]             INT              IDENTITY (1, 1) NOT NULL,
    [uniqueId]       UNIQUEIDENTIFIER CONSTRAINT [DF_umbracoNode_uniqueId] DEFAULT (newid()) NOT NULL,
    [parentId]       INT              NOT NULL,
    [level]          INT              NOT NULL,
    [path]           NVARCHAR (150)   NOT NULL,
    [sortOrder]      INT              NOT NULL,
    [trashed]        BIT              CONSTRAINT [DF_umbracoNode_trashed] DEFAULT ('0') NOT NULL,
    [nodeUser]       INT              NULL,
    [text]           NVARCHAR (255)   NULL,
    [nodeObjectType] UNIQUEIDENTIFIER NULL,
    [createDate]     DATETIME         CONSTRAINT [DF_umbracoNode_createDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_umbracoNode] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoNode_umbracoNode_id] FOREIGN KEY ([parentId]) REFERENCES [dbo].[umbracoNode] ([id]),
    CONSTRAINT [FK_umbracoNode_umbracoUser_id] FOREIGN KEY ([nodeUser]) REFERENCES [dbo].[umbracoUser] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoNode_UniqueId]
    ON [dbo].[umbracoNode]([uniqueId] ASC)
    INCLUDE([parentId], [level], [path], [sortOrder], [trashed], [nodeUser], [text], [createDate]);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoNode_ParentId]
    ON [dbo].[umbracoNode]([parentId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoNode_Level]
    ON [dbo].[umbracoNode]([level] ASC, [parentId] ASC, [sortOrder] ASC, [nodeObjectType] ASC, [trashed] ASC)
    INCLUDE([nodeUser], [path], [uniqueId], [createDate]);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoNode_Path]
    ON [dbo].[umbracoNode]([path] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoNode_Trashed]
    ON [dbo].[umbracoNode]([trashed] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_umbracoNode_ObjectType]
    ON [dbo].[umbracoNode]([nodeObjectType] ASC, [trashed] ASC)
    INCLUDE([uniqueId], [parentId], [level], [path], [sortOrder], [nodeUser], [text], [createDate]);

