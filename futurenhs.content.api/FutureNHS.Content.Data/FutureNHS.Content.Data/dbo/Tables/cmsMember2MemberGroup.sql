CREATE TABLE [dbo].[cmsMember2MemberGroup] (
    [Member]      INT NOT NULL,
    [MemberGroup] INT NOT NULL,
    CONSTRAINT [PK_cmsMember2MemberGroup] PRIMARY KEY CLUSTERED ([Member] ASC, [MemberGroup] ASC),
    CONSTRAINT [FK_cmsMember2MemberGroup_cmsMember_nodeId] FOREIGN KEY ([Member]) REFERENCES [dbo].[cmsMember] ([nodeId]),
    CONSTRAINT [FK_cmsMember2MemberGroup_umbracoNode_id] FOREIGN KEY ([MemberGroup]) REFERENCES [dbo].[umbracoNode] ([id])
);

