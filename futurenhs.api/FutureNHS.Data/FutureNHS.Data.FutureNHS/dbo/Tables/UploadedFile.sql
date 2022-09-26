﻿CREATE TABLE [dbo].[UploadedFile] (
    [Id]                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Filename]          NVARCHAR (200)   NOT NULL,
    [CreatedAtUTC]       DATETIME2         NOT NULL,
    [Entity]           UNIQUEIDENTIFIER NULL,
    [MembershipUser_Id] UNIQUEIDENTIFIER NOT NULL,
    [IsDeleted]         BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]        ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.UploadedFile] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.UploadedFile_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.UploadedFile_dbo.Post_Post_Id] FOREIGN KEY ([Entity]) REFERENCES [dbo].[Entity] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[UploadedFile]([MembershipUser_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Post_Id]
    ON [dbo].[UploadedFile]([Entity] ASC);

