CREATE TABLE [dbo].[FileHistory] (
    [Id]            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [FileId]        UNIQUEIDENTIFIER NOT NULL,
    [Title]         NVARCHAR (200)   NOT NULL,
    [Description]   NVARCHAR (4000)  NULL,
    [FileName]      NVARCHAR (256)   NOT NULL,
    [FileSizeBytes] BIGINT           NULL,
    [FileExtension] NVARCHAR (10)    NULL,
    [BlobName]      NVARCHAR (42)    NULL, 
    [ModifiedBy]    UNIQUEIDENTIFIER NULL,
    [ModifiedAtUtc] DATETIME2 (7)    NULL,
    [FileStatus]    UNIQUEIDENTIFIER NOT NULL,
    [BlobHash]      BINARY (16)      NULL,
    [VersionId]     NVARCHAR (50)    NULL,
    [IsDeleted] BIT  DEFAULT ((0))   NOT NULL, 
    [RowVersion] ROWVERSION          NOT NULL, 
    CONSTRAINT [PK_dbo.FileHistory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.FileHistory_dbo.File_FileId] FOREIGN KEY ([FileId]) REFERENCES [dbo].[File] ([Id]),
    
);

GO
CREATE NONCLUSTERED INDEX [IX_FileId]
    ON [dbo].[FileHistory]([FileId] ASC);

