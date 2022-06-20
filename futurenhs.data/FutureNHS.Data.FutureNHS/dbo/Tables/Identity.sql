CREATE TABLE [dbo].[Identity] (
    [MembershipUser_Id]  UNIQUEIDENTIFIER NOT NULL,
    [Subject_Id]         NVARCHAR (255)   NOT NULL,
    [Issuer]             NVARCHAR (255)   NOT NULL,
    CONSTRAINT [PK_dbo.Identity] PRIMARY KEY CLUSTERED ([MembershipUser_Id] ASC, [Subject_Id])
);

