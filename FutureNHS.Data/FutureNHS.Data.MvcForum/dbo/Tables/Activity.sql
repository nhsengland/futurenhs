CREATE TABLE [dbo].[Activity] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [Type]      NVARCHAR (50)    NOT NULL,
    [Data]      NVARCHAR (MAX)   NOT NULL,
    [Timestamp] DATETIME         NOT NULL,
    CONSTRAINT [PK_dbo.Activity] PRIMARY KEY CLUSTERED ([Id] ASC)
);

