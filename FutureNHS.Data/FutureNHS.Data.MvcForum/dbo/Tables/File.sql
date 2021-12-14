CREATE TABLE [dbo].[File] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [Title]         NVARCHAR (45)    NOT NULL,
    [Description]   NVARCHAR (150)   NULL,
    [FileName]      NVARCHAR (100)   NOT NULL,
    [FileSizeBytes] NVARCHAR (MAX)   NULL,
    [FileExtension] NVARCHAR (10)    NULL,
    [BlobName]      NVARCHAR (42)    NULL,
    [CreatedBy]     UNIQUEIDENTIFIER NOT NULL,
    [ModifiedBy]    UNIQUEIDENTIFIER NULL,
    [CreatedAtUtc]  DATETIME2 (7)    NOT NULL,
    [ModifiedAtUtc] DATETIME2 (7)    NULL,
    [ParentFolder]  UNIQUEIDENTIFIER DEFAULT ('00000000-0000-0000-0000-000000000000') NOT NULL,
    [FileStatus]    INT              NOT NULL,
    [BlobHash]      BINARY (16)      NULL,
    CONSTRAINT [PK_dbo.File] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.File_dbo.Folder_ParentFolder] FOREIGN KEY ([ParentFolder]) REFERENCES [dbo].[Folder] ([Id]),
    CONSTRAINT [FK_dbo.File_dbo.MembershipUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.File_dbo.MembershipUser_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.File_dbo.UploadStatus_UploadStatus] FOREIGN KEY ([FileStatus]) REFERENCES [dbo].[FileStatus] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ParentFolder]
    ON [dbo].[File]([ParentFolder] ASC);

