CREATE TABLE [dbo].[umbracoAudit] (
    [id]                INT             IDENTITY (1, 1) NOT NULL,
    [performingUserId]  INT             NOT NULL,
    [performingDetails] NVARCHAR (1024) NULL,
    [performingIp]      NVARCHAR (64)   NULL,
    [eventDateUtc]      DATETIME        CONSTRAINT [DF_umbracoAudit_eventDateUtc] DEFAULT (getdate()) NOT NULL,
    [affectedUserId]    INT             NOT NULL,
    [affectedDetails]   NVARCHAR (1024) NULL,
    [eventType]         NVARCHAR (256)  NOT NULL,
    [eventDetails]      NVARCHAR (1024) NULL,
    CONSTRAINT [PK_umbracoAudit] PRIMARY KEY CLUSTERED ([id] ASC)
);

