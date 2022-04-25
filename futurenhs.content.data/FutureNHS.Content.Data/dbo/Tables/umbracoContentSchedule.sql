CREATE TABLE [dbo].[umbracoContentSchedule] (
    [id]         UNIQUEIDENTIFIER NOT NULL,
    [nodeId]     INT              NOT NULL,
    [languageId] INT              NULL,
    [date]       DATETIME         NOT NULL,
    [action]     NVARCHAR (255)   NOT NULL,
    CONSTRAINT [PK_umbracoContentSchedule] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_umbracoContentSchedule_umbracoContent_nodeId] FOREIGN KEY ([nodeId]) REFERENCES [dbo].[umbracoContent] ([nodeId]),
    CONSTRAINT [FK_umbracoContentSchedule_umbracoLanguage_id] FOREIGN KEY ([languageId]) REFERENCES [dbo].[umbracoLanguage] ([id])
);

