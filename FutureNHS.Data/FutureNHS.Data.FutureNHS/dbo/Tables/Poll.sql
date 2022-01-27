CREATE TABLE [dbo].[Poll] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [IsClosed]           BIT              NOT NULL,
    [DateCreated]        DATETIME         NOT NULL,
    [ClosePollAfterDays] INT              NULL,
    [ExtendedDataString] NVARCHAR (MAX)   NULL,
    [MembershipUser_Id]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.Poll] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Poll_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Poll]([MembershipUser_Id] ASC);

