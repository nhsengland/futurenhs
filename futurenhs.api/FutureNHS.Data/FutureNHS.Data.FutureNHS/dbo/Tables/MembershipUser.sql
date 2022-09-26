CREATE TABLE [dbo].[MembershipUser] (
    [Id]                             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserName]                       NVARCHAR (150)   NOT NULL,
    [Email]                          NVARCHAR (256)   NULL,
    [IsApproved]                     BIT              NOT NULL,
    [IsLockedOut]                    BIT              NOT NULL,
    [IsBanned]                       BIT              NOT NULL,
    [CreatedAtUTC]                   DATETIME2        NOT NULL,
    [ModifiedAtUTC]                  DATETIME2        NULL,
    [LastLoginDateUTC]               DATETIME2        NULL,
    [Slug]                           NVARCHAR (150)   NOT NULL,
    [HasAgreedToTermsAndConditions]  BIT              NOT NULL,
    [FirstName]                      NVARCHAR (255)   NULL,
    [Surname]                        NVARCHAR (255)   NULL,
    [Initials]                       NVARCHAR (2)     NULL,
    [Pronouns]                       NVARCHAR (255)   NULL,
    [ImageId]                        UNIQUEIDENTIFIER NULL, 
    [IsDeleted]                      BIT  DEFAULT ((0)) NOT NULL, 
    [RowVersion]                     ROWVERSION NOT NULL, 
    CONSTRAINT [PK_dbo.MembershipUser] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.MembershipUser_dbo.Image_MembershipUserImage] FOREIGN KEY ([ImageId]) REFERENCES [dbo].[Image] ([Id]),
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_MembershipUser_Slug]
    ON [dbo].[MembershipUser]([Slug] ASC);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_MembershipUser_UserName]
    ON [dbo].[MembershipUser]([UserName] ASC);
