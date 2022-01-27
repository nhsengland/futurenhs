CREATE TABLE [dbo].[SystemPage] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [Slug]      NVARCHAR (50)    NOT NULL,
    [Title]     NVARCHAR (100)   NOT NULL,
    [Content]   NVARCHAR (MAX)   NOT NULL,
    [IsDeleted] BIT              NOT NULL,
    CONSTRAINT [PK_dbo.SystemPage] PRIMARY KEY CLUSTERED ([Id] ASC)
);

