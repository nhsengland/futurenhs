CREATE TABLE [dbo].[Entity_Like] (
    [Entity_Id]                UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id]        UNIQUEIDENTIFIER NOT NULL,
    [CreatedAtUTC]             DATETIME2         NOT NULL,   
    [RowVersion]               ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.Entity_Like] PRIMARY KEY CLUSTERED ([Entity_Id], [MembershipUser_Id] ASC),
    CONSTRAINT [FK_dbo.Entity_Like_dbo.MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
 );

GO
CREATE NONCLUSTERED INDEX [IX_Entity_Like_MembershipUser_Id]
    ON [dbo].[Entity_Like]([MembershipUser_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Entity_Like_Id]
    ON [dbo].[Entity_Like]([Entity_Id] ASC);

