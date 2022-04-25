CREATE TABLE [dbo].[cmsMemberType] (
    [pk]             INT IDENTITY (1, 1) NOT NULL,
    [NodeId]         INT NOT NULL,
    [propertytypeId] INT NOT NULL,
    [memberCanEdit]  BIT CONSTRAINT [DF_cmsMemberType_memberCanEdit] DEFAULT ('0') NOT NULL,
    [viewOnProfile]  BIT CONSTRAINT [DF_cmsMemberType_viewOnProfile] DEFAULT ('0') NOT NULL,
    [isSensitive]    BIT CONSTRAINT [DF_cmsMemberType_isSensitive] DEFAULT ('0') NOT NULL,
    CONSTRAINT [PK_cmsMemberType] PRIMARY KEY CLUSTERED ([pk] ASC),
    CONSTRAINT [FK_cmsMemberType_cmsContentType_nodeId] FOREIGN KEY ([NodeId]) REFERENCES [dbo].[cmsContentType] ([nodeId]),
    CONSTRAINT [FK_cmsMemberType_umbracoNode_id] FOREIGN KEY ([NodeId]) REFERENCES [dbo].[umbracoNode] ([id])
);

