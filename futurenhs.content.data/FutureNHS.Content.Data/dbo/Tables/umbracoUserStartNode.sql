CREATE TABLE [dbo].[umbracoUserStartNode] (
    [id]            INT IDENTITY (1, 1) NOT NULL,
    [userId]        INT NOT NULL,
    [startNode]     INT NOT NULL,
    [startNodeType] INT NOT NULL,
    CONSTRAINT [PK_userStartNode] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoUserStartNode_umbracoNode_id] FOREIGN KEY ([startNode]) REFERENCES [dbo].[umbracoNode] ([id]),
    CONSTRAINT [FK_umbracoUserStartNode_umbracoUser_id] FOREIGN KEY ([userId]) REFERENCES [dbo].[umbracoUser] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoUserStartNode_startNodeType]
    ON [dbo].[umbracoUserStartNode]([startNodeType] ASC, [startNode] ASC, [userId] ASC);

