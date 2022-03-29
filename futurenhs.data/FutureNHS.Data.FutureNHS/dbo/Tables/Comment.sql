CREATE TABLE [dbo].[Comment] (
    [Entity_Id]             UNIQUEIDENTIFIER    NOT NULL, 
    [Content]               NVARCHAR (MAX)      NOT NULL,
    [CreatedBy]             UNIQUEIDENTIFIER    NOT NULL,
    [CreatedAtUTC]          DATETIME2           NOT NULL,
    [ModifiedBy]            UNIQUEIDENTIFIER    NULL,
    [ModifiedAtUTC]         DATETIME2           NULL,
    [FlaggedAsSpam]         BIT                 NULL,
    [InReplyTo]             UNIQUEIDENTIFIER    NULL,
    [ThreadId]              UNIQUEIDENTIFIER    NULL,
    [IsDeleted]             BIT  DEFAULT ((0))  NOT NULL, 
    [RowVersion]            ROWVERSION          NOT NULL,    
    [Parent_EntityId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [FK_dbo.Comment_dbo.Entity_Id] FOREIGN KEY ([Entity_Id]) REFERENCES [dbo].[Entity] ([Id]),
    CONSTRAINT [FK_dbo.Comment_dbo.Parent_EntityId] FOREIGN KEY ([Parent_EntityId]) REFERENCES [dbo].[Entity] ([Id]),
    CONSTRAINT [FK_dbo.Comment_dbo.InReplyTo] FOREIGN KEY ([InReplyTo]) REFERENCES [dbo].[Comment] ([Entity_Id]),
    CONSTRAINT [FK_dbo.Comment_dbo.ThreadId] FOREIGN KEY ([ThreadId]) REFERENCES [dbo].[Comment] ([Entity_Id]),   
    CONSTRAINT [FK_dbo.Comment_dbo.MembershipUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Comment_dbo.MembershipUser_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[MembershipUser] ([Id]), 
    CONSTRAINT [PK_Comment] PRIMARY KEY ([Entity_Id]), 
);

GO
CREATE NONCLUSTERED INDEX [IX_Comment_Id]
    ON [dbo].[Comment]([Entity_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Comment_Parent_Id]
    ON [dbo].[Comment]([Parent_EntityId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Comment]([CreatedBy] ASC);

