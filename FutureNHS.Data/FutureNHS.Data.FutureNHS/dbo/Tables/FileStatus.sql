CREATE TABLE [dbo].[FileStatus] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (20) NULL,
    CONSTRAINT [PK_dbo.FileStatus] PRIMARY KEY CLUSTERED ([Id] ASC)
);

