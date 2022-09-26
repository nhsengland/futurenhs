CREATE TABLE [dbo].[GroupSite]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [GroupId] UNIQUEIDENTIFIER NOT NULL, 
    [ContentRootId] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedBy]             UNIQUEIDENTIFIER    NULL,
    [CreatedAtUTC]          DATETIME2           NOT NULL,
    [ModifiedBy]            UNIQUEIDENTIFIER    NULL,
    [ModifiedAtUTC]         DATETIME2           NULL,
    CONSTRAINT [FK_dbo.GroupId_dbo.Group_Id] FOREIGN KEY ([GroupId]) REFERENCES [Group]([Id])
)
