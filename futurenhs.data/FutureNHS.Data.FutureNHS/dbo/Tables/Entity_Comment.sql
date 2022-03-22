CREATE TABLE [dbo].[Entity_Comment] (
    [Id]                    UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
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
    CONSTRAINT [PK_dbo.Entity_Comment] PRIMARY KEY CLUSTERED ([Id], [Entity_Id] ASC),
    CONSTRAINT [FK_dbo.Entity_Comment_dbo.Entity_Id] FOREIGN KEY ([Entity_Id]) REFERENCES [dbo].[Entity] ([Id]),
    CONSTRAINT [FK_dbo.Entity_Comment_dbo.MembershipUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Entity_Comment_dbo.MembershipUser_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
);

GO
CREATE NONCLUSTERED INDEX [IX_Comment_Id]
    ON [dbo].[Entity_Comment]([Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Entity_Comment]([CreatedBy] ASC);

