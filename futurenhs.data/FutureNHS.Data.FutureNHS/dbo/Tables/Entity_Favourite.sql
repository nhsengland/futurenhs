CREATE TABLE [dbo].[Entity_Favourite] (
    [Entity_Id]                UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id]        UNIQUEIDENTIFIER NOT NULL,
    [CreatedAtUTC]             DATETIME2         NOT NULL,   
    [RowVersion]               ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.Entity_Favourite] PRIMARY KEY CLUSTERED ([Entity_Id], [MembershipUser_Id] ASC),
    CONSTRAINT [FK_dbo.Entity_Favourite_dbo.MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
 );

GO
CREATE NONCLUSTERED INDEX [IX_Entity_Favourite_MembershipUser_Id]
    ON [dbo].[Entity_Favourite]([MembershipUser_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Entity_Favourite_Id]
    ON [dbo].[Entity_Favourite]([Entity_Id] ASC);

