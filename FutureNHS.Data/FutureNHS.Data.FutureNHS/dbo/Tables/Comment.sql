﻿CREATE TABLE [dbo].[Comment] (
    [Id]                    UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    [Content]               NVARCHAR (MAX)      NOT NULL,
    [CreatedBy]             UNIQUEIDENTIFIER    NOT NULL,
    [CreatedAtUTC]          DATETIME2           NOT NULL,
    [ModifiedBy]            UNIQUEIDENTIFIER    NULL,
    [ModifiedAtUTC]         DATETIME2           NULL,
    [LikeCount]             INT                 NOT NULL DEFAULT ((0)),
    [FlaggedAsSpam]         BIT                 NULL,
    [InReplyTo]             UNIQUEIDENTIFIER    NULL,
    [Discussion_Id]         UNIQUEIDENTIFIER    NOT NULL,
    [ThreadId]              UNIQUEIDENTIFIER    NULL,
    [IsDeleted]             BIT  DEFAULT ((0))  NOT NULL, 
    [RowVersion]            ROWVERSION          NOT NULL, 
    CONSTRAINT [PK_dbo.Comment] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Comment_dbo.MembershipUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Comment_dbo.MembershipUser_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.Comment_dbo.Discussion_Discussion_Id] FOREIGN KEY ([Discussion_Id]) REFERENCES [dbo].[Discussion] ([Id]),
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Comment]([CreatedBy] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Discussion_Id]
    ON [dbo].[Comment]([Discussion_Id] ASC);
