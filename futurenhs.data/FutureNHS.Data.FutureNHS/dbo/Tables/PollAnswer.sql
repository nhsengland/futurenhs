CREATE TABLE [dbo].[PollAnswer] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Answer]             NVARCHAR (600)   NOT NULL,
    [Poll_Id]            UNIQUEIDENTIFIER NOT NULL,
    [CreatedAtUTC]       DATETIME2 NOT NULL,
    [IsDeleted]          BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]         ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.PollAnswer] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.PollAnswer_dbo.Poll_Poll_Id] FOREIGN KEY ([Poll_Id]) REFERENCES [dbo].[Poll] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Poll_Id]
    ON [dbo].[PollAnswer]([Poll_Id] ASC);

