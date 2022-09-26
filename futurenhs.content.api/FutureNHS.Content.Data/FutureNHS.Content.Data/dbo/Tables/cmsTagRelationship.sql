CREATE TABLE [dbo].[cmsTagRelationship] (
    [nodeId]         INT NOT NULL,
    [tagId]          INT NOT NULL,
    [propertyTypeId] INT NOT NULL,
    CONSTRAINT [PK_cmsTagRelationship] PRIMARY KEY CLUSTERED ([nodeId] ASC, [propertyTypeId] ASC, [tagId] ASC),
    CONSTRAINT [FK_cmsTagRelationship_cmsContent] FOREIGN KEY ([nodeId]) REFERENCES [dbo].[umbracoContent] ([nodeId]),
    CONSTRAINT [FK_cmsTagRelationship_cmsPropertyType] FOREIGN KEY ([propertyTypeId]) REFERENCES [dbo].[cmsPropertyType] ([id]),
    CONSTRAINT [FK_cmsTagRelationship_cmsTags_id] FOREIGN KEY ([tagId]) REFERENCES [dbo].[cmsTags] ([id])
);

