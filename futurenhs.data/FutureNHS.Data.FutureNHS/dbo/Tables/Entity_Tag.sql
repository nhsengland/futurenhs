CREATE TABLE [dbo].[Entity_Tag] (
    [Entity_Id]    UNIQUEIDENTIFIER NOT NULL,
    [Tag_Id] UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id] UNIQUEIDENTIFIER NULL,
    [RowVersion]         ROWVERSION NOT NULL, 
  
    CONSTRAINT [PK_dbo.Entity_Tag] PRIMARY KEY CLUSTERED ([Entity_Id] ASC, [Tag_Id] ASC),
    CONSTRAINT [FK_dbo.Entity_Tag_dbo.Entity_Id] FOREIGN KEY ([Entity_Id]) REFERENCES [dbo].[Discussion] ([Entity_Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.Entity_Tag_dbo.Tag_Id] FOREIGN KEY ([Tag_Id]) REFERENCES [dbo].[Tag] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.Entity_Tag_dbo.MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Entity_Id]
    ON [dbo].[Entity_Tag]([Entity_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Tag_Id]
    ON [dbo].[Entity_Tag]([Tag_Id] ASC);

