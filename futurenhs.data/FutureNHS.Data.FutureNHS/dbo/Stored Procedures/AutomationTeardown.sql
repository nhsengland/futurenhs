CREATE PROC [dbo].[AutomationTeardown]
AS
	-- Disable constraints for all tables:
    EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'

    SELECT [Id] INTO #autoUsers FROM [dbo].[MembershipUser] WHERE [Email] IN ('autoAdmin@test.co.uk', 'autoGroupAdmin@test.co.uk', 'autoRemoveUser@test.co.uk', 'autoUser@test.co.uk', 'autoUser2@test.co.uk', 'autoUser3@test.co.uk', 'autoTest@email.com', 'VisRegUser@email.com', 'autoEditUser@test.co.uk');
	DECLARE @groupAdmin AS uniqueidentifier;
    SELECT @groupAdmin = id FROM [dbo].[MembershipUser] WHERE Email = 'autoGroupAdmin@test.co.uk';
    DECLARE @autoUser AS uniqueidentifier;
    SELECT @autoUser = id FROM [dbo].[MembershipUser] WHERE Email = 'autoUser@test.co.uk';

	DECLARE @entityTable TABLE (id VARCHAR(50))
	INSERT INTO @entityTable SELECT Entity_Id FROM [dbo].[Comment] WHERE CreatedBy IN ( SELECT * FROM #autoUsers);
    INSERT INTO @entityTable SELECT Entity_Id FROM [dbo].[Discussion] WHERE CreatedBy IN ( SELECT * FROM #autoUsers);
	
    DELETE FROM [dbo].[GroupSite] 
	WHERE [GroupId] IN (
		SELECT Id FROM [dbo].[Group]
		WHERE [GroupOwner] IN (
			SELECT * FROM #autoUsers
		)
	);

	DELETE FROM [dbo].[Image]
	WHERE [CreatedBy] IN (
		 SELECT [Id] FROM #autoUsers
	);

	DELETE FROM [dbo].[Entity] 
    WHERE [Id] IN (
        SELECT * FROM @entityTable
    );

    DELETE FROM [dbo].[File] 
    WHERE [CreatedBy] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[Folder] 
    WHERE [CreatedBy] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[GroupUser] 
    WHERE [Group_Id] IN (
        SELECT [Id] FROM [dbo].[Group] WHERE [GroupOwner] IN (SELECT * FROM #autoUsers)
    );

    DELETE FROM [dbo].[MembershipUsersInRoles]
    WHERE [UserIdentifier] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[SystemPage]
    WHERE [Slug] = 'repeat-slug-test';

    DELETE FROM [dbo].[Comment]
    WHERE [CreatedBy] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[Discussion]
    WHERE [CreatedBy] IN (
        SELECT * FROM #autoUsers
    );
	
	DELETE FROM [dbo].[Group] 
    WHERE [GroupOwner] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[Identity] 
    WHERE [MembershipUser_Id] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[MembershipUser]
    WHERE [Id] IN (
        SELECT * FROM #autoUsers
    );
    
	DROP TABLE #autoUsers

    -- Re-enable constraints for all tables:
    EXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all';	