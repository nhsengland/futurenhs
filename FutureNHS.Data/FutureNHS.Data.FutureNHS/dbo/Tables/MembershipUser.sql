CREATE TABLE [dbo].[MembershipUser] (
    [Id]                            UNIQUEIDENTIFIER NOT NULL,
    [UserName]                      NVARCHAR (150)   NOT NULL,
    [Password]                      NVARCHAR (128)   NOT NULL,
    [PasswordSalt]                  NVARCHAR (128)   NULL,
    [Email]                         NVARCHAR (256)   NULL,
    [PasswordQuestion]              NVARCHAR (256)   NULL,
    [PasswordAnswer]                NVARCHAR (256)   NULL,
    [IsApproved]                    BIT              NOT NULL,
    [IsLockedOut]                   BIT              NOT NULL,
    [IsBanned]                      BIT              NOT NULL,
    [CreateDate]                    DATETIME         NOT NULL,
    [LastLoginDate]                 DATETIME         NULL,
    [LastPasswordChangedDate]       DATETIME         NULL,
    [LastLockoutDate]               DATETIME         NULL,
    [LastActivityDate]              DATETIME         NULL,
    [FailedPasswordAttemptCount]    INT              NOT NULL,
    [FailedPasswordAnswerAttempt]   INT              NOT NULL,
    [PasswordResetToken]            NVARCHAR (150)   NULL,
    [PasswordResetTokenCreatedAt]   DATETIME         NULL,
    [Comment]                       NVARCHAR (MAX)   NULL,
    [Slug]                          NVARCHAR (150)   NOT NULL,
    [Signature]                     NVARCHAR (1000)  NULL,
    [Age]                           INT              NULL,
    [Location]                      NVARCHAR (100)   NULL,
    [Website]                       NVARCHAR (100)   NULL,
    [Twitter]                       NVARCHAR (60)    NULL,
    [Facebook]                      NVARCHAR (60)    NULL,
    [Avatar]                        NVARCHAR (500)   NULL,
    [FacebookAccessToken]           NVARCHAR (1000)  NULL,
    [FacebookId]                    BIGINT           NULL,
    [TwitterAccessToken]            NVARCHAR (1000)  NULL,
    [TwitterId]                     NVARCHAR (150)   NULL,
    [GoogleAccessToken]             NVARCHAR (1000)  NULL,
    [GoogleId]                      NVARCHAR (150)   NULL,
    [MicrosoftAccessToken]          NVARCHAR (1000)  NULL,
    [MicrosoftId]                   NVARCHAR (MAX)   NULL,
    [IsExternalAccount]             BIT              NULL,
    [TwitterShowFeed]               BIT              NULL,
    [LoginIdExpires]                DATETIME         NULL,
    [MiscAccessToken]               NVARCHAR (250)   NULL,
    [DisableEmailNotifications]     BIT              NULL,
    [DisablePosting]                BIT              NULL,
    [DisablePrivateMessages]        BIT              NULL,
    [DisableFileUploads]            BIT              NULL,
    [HasAgreedToTermsAndConditions] BIT              NULL,
    [IsTrustedUser]                 BIT              NOT NULL,
    [ExtendedDataString]            NVARCHAR (MAX)   NULL,
    [FirstName]                     NVARCHAR (255)   NULL,
    [Surname]                       NVARCHAR (255)   NULL,
    [Initials]                      NVARCHAR (2)     NULL,
    [Pronouns]                      NVARCHAR (255)   NULL,
    [ImageId]                       UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [PK_dbo.MembershipUser] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.MembershipUser_dbo.Image_MembershipUserImage] FOREIGN KEY ([ImageId]) REFERENCES [dbo].[Image] ([Id]),
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_MembershipUser_Slug]
    ON [dbo].[MembershipUser]([Slug] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_MembershipUser_UserName]
    ON [dbo].[MembershipUser]([UserName] ASC);

