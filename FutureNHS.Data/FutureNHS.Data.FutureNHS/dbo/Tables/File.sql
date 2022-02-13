CREATE TABLE [dbo].[File] (
    [Id]            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Title]         NVARCHAR (45)    NOT NULL,
    [Description]   NVARCHAR (150)   NULL,
    [FileName]      NVARCHAR (100)   NOT NULL,
    [FileSizeBytes] BIGINT   NULL,
    [FileExtension] NVARCHAR (10)    NULL,
    [BlobName]      NVARCHAR (42)    NULL,
    [CreatedBy]     UNIQUEIDENTIFIER NOT NULL,
    [ModifiedBy]    UNIQUEIDENTIFIER NULL,
    [CreatedAtUtc]  DATETIME2 (7)    NOT NULL,
    [ModifiedAtUtc] DATETIME2 (7)    NULL,
    [ParentFolder]  UNIQUEIDENTIFIER NOT NULL,
    [FileStatus]    UNIQUEIDENTIFIER              NOT NULL,
    [BlobHash]      BINARY (16)      NULL,
    [IsDeleted] BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion] ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.File] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.File_dbo.Folder_ParentFolder] FOREIGN KEY ([ParentFolder]) REFERENCES [dbo].[Folder] ([Id]),
    CONSTRAINT [FK_dbo.File_dbo.MembershipUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.File_dbo.MembershipUser_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.File_dbo.UploadStatus_UploadStatus] FOREIGN KEY ([FileStatus]) REFERENCES [dbo].[FileStatus] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ParentFolder]
    ON [dbo].[File]([ParentFolder] ASC);

