CREATE TABLE [dbo].[Category] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (200)   NOT NULL,
    [Description]        NVARCHAR (4000)   NOT NULL,
    [SortOrder]          INT              NOT NULL,
    [CreatedAtUTC]       DATETIME2         NOT NULL,
    [Group_Id]           UNIQUEIDENTIFIER NOT NULL,
    [IsDeleted]          BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]         ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.Category] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Category_dbo.Group_Group_Id] FOREIGN KEY ([Group_Id]) REFERENCES [dbo].[Group] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Group_Id]
    ON [dbo].[Category]([Group_Id] ASC);

