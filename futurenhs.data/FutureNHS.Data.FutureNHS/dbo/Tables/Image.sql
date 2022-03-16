CREATE TABLE [dbo].[Image] (
    [Id]            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [FileName]      NVARCHAR (45)    NOT NULL,
    [FileSizeBytes] INT              NOT NULL,
    [Height]        INT              NOT NULL,
    [Width]         INT              NOT NULL,
    [MediaType]     NVARCHAR (10)    NOT NULL,
    [IsDeleted]     BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]    ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.Image] PRIMARY KEY CLUSTERED ([Id] ASC)
);

