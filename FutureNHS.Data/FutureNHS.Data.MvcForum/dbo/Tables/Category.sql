CREATE TABLE [dbo].[Category] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (MAX)   NOT NULL,
    [Description]        NVARCHAR (MAX)   NOT NULL,
    [SortOrder]          INT              NOT NULL,
    [DateCreated]        DATETIME         NOT NULL,
    [ExtendedDataString] NVARCHAR (MAX)   NULL,
    [Group_Id]           UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.Category] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Category_dbo.Group_Group_Id] FOREIGN KEY ([Group_Id]) REFERENCES [dbo].[Group] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Group_Id]
    ON [dbo].[Category]([Group_Id] ASC);

