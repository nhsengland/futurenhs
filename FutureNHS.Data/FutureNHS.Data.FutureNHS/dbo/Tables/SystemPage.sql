CREATE TABLE [dbo].[SystemPage] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [Slug]      NVARCHAR (50)    NOT NULL,
    [Title]     NVARCHAR (450)   NOT NULL,
    [Content]   NVARCHAR (MAX)   NOT NULL,
    [IsDeleted] BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion] ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.SystemPage] PRIMARY KEY CLUSTERED ([Id] ASC)
);

