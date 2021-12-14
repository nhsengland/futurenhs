CREATE TABLE [dbo].[MembershipUserPoints] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [Points]            INT              NOT NULL,
    [DateAdded]         DATETIME         NOT NULL,
    [PointsFor]         INT              NOT NULL,
    [PointsForId]       UNIQUEIDENTIFIER NULL,
    [Notes]             NVARCHAR (400)   NULL,
    [MembershipUser_Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.MembershipUserPoints] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.MembershipUserPoints_dbo.MembershipUser_MembershipUser_Id] FOREIGN KEY ([MembershipUser_Id]) REFERENCES [dbo].[MembershipUser] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUser_Id]
    ON [dbo].[MembershipUserPoints]([MembershipUser_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUserPoints_PointsFor]
    ON [dbo].[MembershipUserPoints]([PointsFor] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipUserPoints_PointsForId]
    ON [dbo].[MembershipUserPoints]([PointsForId] ASC);

