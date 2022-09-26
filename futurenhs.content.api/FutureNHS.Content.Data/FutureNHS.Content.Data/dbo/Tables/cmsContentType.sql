CREATE TABLE [dbo].[cmsContentType] (
    [pk]          INT             IDENTITY (1, 1) NOT NULL,
    [nodeId]      INT             NOT NULL,
    [alias]       NVARCHAR (255)  NULL,
    [icon]        NVARCHAR (255)  NULL,
    [thumbnail]   NVARCHAR (255)  CONSTRAINT [DF_cmsContentType_thumbnail] DEFAULT ('folder.png') NOT NULL,
    [description] NVARCHAR (1500) NULL,
    [isContainer] BIT             CONSTRAINT [DF_cmsContentType_isContainer] DEFAULT ('0') NOT NULL,
    [isElement]   BIT             CONSTRAINT [DF_cmsContentType_isElement] DEFAULT ('0') NOT NULL,
    [allowAtRoot] BIT             CONSTRAINT [DF_cmsContentType_allowAtRoot] DEFAULT ('0') NOT NULL,
    [variations]  INT             CONSTRAINT [DF_cmsContentType_variations] DEFAULT ('1') NOT NULL,
    CONSTRAINT [PK_cmsContentType] PRIMARY KEY CLUSTERED ([pk] ASC),
    CONSTRAINT [FK_cmsContentType_umbracoNode_id] FOREIGN KEY ([nodeId]) REFERENCES [dbo].[umbracoNode] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_cmsContentType]
    ON [dbo].[cmsContentType]([nodeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_cmsContentType_icon]
    ON [dbo].[cmsContentType]([icon] ASC);

