CREATE TABLE [dbo].[PollAnswer] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [Answer]             NVARCHAR (600)   NOT NULL,
    [ExtendedDataString] NVARCHAR (MAX)   NULL,
    [Poll_Id]            UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.PollAnswer] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.PollAnswer_dbo.Poll_Poll_Id] FOREIGN KEY ([Poll_Id]) REFERENCES [dbo].[Poll] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Poll_Id]
    ON [dbo].[PollAnswer]([Poll_Id] ASC);

