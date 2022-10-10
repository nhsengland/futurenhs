CREATE TABLE [dbo].[ApprovedDomain] (
    [Id]        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [EmailDomain]     NVARCHAR (200)   NOT NULL,
    [IsDeleted] BIT  DEFAULT ((0)) NOT NULL,
    [RowVersion] ROWVERSION NOT NULL,
    CONSTRAINT [PK_dbo.ApprovedDomain] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ApprovedDomain_EmailDomain]
    ON [dbo].[ApprovedDomain]([EmailDomain] ASC);