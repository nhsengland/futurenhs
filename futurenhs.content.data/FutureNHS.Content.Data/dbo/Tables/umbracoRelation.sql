CREATE TABLE [dbo].[umbracoRelation] (
    [id]       INT             IDENTITY (1, 1) NOT NULL,
    [parentId] INT             NOT NULL,
    [childId]  INT             NOT NULL,
    [relType]  INT             NOT NULL,
    [datetime] DATETIME        CONSTRAINT [DF_umbracoRelation_datetime] DEFAULT (getdate()) NOT NULL,
    [comment]  NVARCHAR (1000) NOT NULL,
    CONSTRAINT [PK_umbracoRelation] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoRelation_umbracoNode] FOREIGN KEY ([parentId]) REFERENCES [dbo].[umbracoNode] ([id]),
    CONSTRAINT [FK_umbracoRelation_umbracoNode1] FOREIGN KEY ([childId]) REFERENCES [dbo].[umbracoNode] ([id]),
    CONSTRAINT [FK_umbracoRelation_umbracoRelationType_id] FOREIGN KEY ([relType]) REFERENCES [dbo].[umbracoRelationType] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_umbracoRelation_parentChildType]
    ON [dbo].[umbracoRelation]([parentId] ASC, [childId] ASC, [relType] ASC);

