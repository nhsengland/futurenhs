CREATE TABLE [dbo].[PostEdit] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [DateEdited]          DATETIME         NOT NULL,
    [OriginalPostContent] NVARCHAR (MAX)   NULL,
    [EditedPostContent]   NVARCHAR (MAX)   NULL,
    [OriginalPostTitle]   NVARCHAR (500)   NULL,
    [EditedPostTitle]     NVARCHAR (500)   NULL,
    [Post_Id]             UNIQUEIDENTIFIER NOT NULL,
    [MembershipUser_Id]   UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.PostEdit] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.PostEdit_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id]),
    CONSTRAINT [FK_dbo.PostEdit_dbo.Post_Post_Id] FOREIGN KEY ([Post_Id]) REFERENCES [dbo].[Post] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[PostEdit]([MembershipUser_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Post_Id]
    ON [dbo].[PostEdit]([Post_Id] ASC);

