﻿CREATE TABLE [dbo].[Settings] (
    [Id]                              UNIQUEIDENTIFIER NOT NULL,
    [ForumName]                       NVARCHAR (500)   NULL,
    [ForumUrl]                        NVARCHAR (500)   NULL,
    [PageTitle]                       NVARCHAR (80)    NULL,
    [MetaDesc]                        NVARCHAR (200)   NULL,
    [IsClosed]                        BIT              NULL,
    [EnableRSSFeeds]                  BIT              NULL,
    [DisplayEditedBy]                 BIT              NULL,
    [EnablePostFileAttachments]       BIT              NULL,
    [EnableMarkAsSolution]            BIT              NULL,
    [MarkAsSolutionReminderTimeFrame] INT              NULL,
    [EnableSpamReporting]             BIT              NULL,
    [EnableMemberReporting]           BIT              NULL,
    [EnableEmailSubscriptions]        BIT              NULL,
    [ManuallyAuthoriseNewMembers]     BIT              NULL,
    [NewMemberEmailConfirmation]      BIT              NULL,
    [EmailAdminOnNewMemberSignUp]     BIT              NULL,
    [TopicsPerPage]                   INT              NULL,
    [PostsPerPage]                    INT              NULL,
    [ActivitiesPerPage]               INT              NULL,
    [EnablePrivateMessages]           BIT              NULL,
    [MaxPrivateMessagesPerMember]     INT              NULL,
    [PrivateMessageFloodControl]      INT              NULL,
    [EnableSignatures]                BIT              NULL,
    [EnablePoints]                    BIT              NULL,
    [PointsAllowedForExtendedProfile] INT              NULL,
    [PointsAllowedToVoteAmount]       INT              NULL,
    [PointsAddedPerPost]              INT              NULL,
    [PointsAddedPostiveVote]          INT              NULL,
    [PointsDeductedNagativeVote]      INT              NULL,
    [PointsAddedForSolution]          INT              NULL,
    [AdminEmailAddress]               NVARCHAR (100)   NULL,
    [NotificationReplyEmail]          NVARCHAR (100)   NULL,
    [SMTP]                            NVARCHAR (100)   NULL,
    [SMTPUsername]                    NVARCHAR (100)   NULL,
    [SMTPPassword]                    NVARCHAR (100)   NULL,
    [SMTPPort]                        NVARCHAR (10)    NULL,
    [SMTPEnableSSL]                   BIT              NULL,
    [Theme]                           NVARCHAR (100)   NULL,
    [EnableSocialLogins]              BIT              NULL,
    [SpamQuestion]                    NVARCHAR (500)   NULL,
    [SpamAnswer]                      NVARCHAR (500)   NULL,
    [EnableAkisment]                  BIT              NULL,
    [AkismentKey]                     NVARCHAR (100)   NULL,
    [CurrentDatabaseVersion]          NVARCHAR (10)    NULL,
    [EnablePolls]                     BIT              NULL,
    [SuspendRegistration]             BIT              NULL,
    [CustomHeaderCode]                NVARCHAR (MAX)   NULL,
    [CustomFooterCode]                NVARCHAR (MAX)   NULL,
    [EnableEmoticons]                 BIT              NULL,
    [DisableDislikeButton]            BIT              NOT NULL,
    [AgreeToTermsAndConditions]       BIT              NULL,
    [TermsAndConditions]              NVARCHAR (MAX)   NULL,
    [DisableStandardRegistration]     BIT              NULL,
    [ExtendedDataString]              NVARCHAR (MAX)   NULL,
    [DefaultLanguage_Id]              UNIQUEIDENTIFIER NOT NULL,
    [NewMemberStartingRole]           UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.Settings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Settings_dbo.Language_DefaultLanguage_Id] FOREIGN KEY ([DefaultLanguage_Id]) REFERENCES [dbo].[Language] ([Id]),
    CONSTRAINT [FK_dbo.Settings_dbo.MembershipRole_NewMemberStartingRole] FOREIGN KEY ([NewMemberStartingRole]) REFERENCES [dbo].[MembershipRole] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_DefaultLanguage_Id]
    ON [dbo].[Settings]([DefaultLanguage_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_NewMemberStartingRole]
    ON [dbo].[Settings]([NewMemberStartingRole] ASC);

