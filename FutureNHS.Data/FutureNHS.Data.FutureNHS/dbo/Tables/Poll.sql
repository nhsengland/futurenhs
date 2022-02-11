CREATE TABLE [dbo].[Poll] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [IsClosed]           BIT              NOT NULL,
    [CreatedAtUTC]        DATETIME         NOT NULL,
    [ClosePollAfterDays] INT              NULL,
    [MembershipUser_Id]  UNIQUEIDENTIFIER NOT NULL,
    [IsDeleted]          BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]         ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.Poll] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Poll_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[Poll]([MembershipUser_Id] ASC);

