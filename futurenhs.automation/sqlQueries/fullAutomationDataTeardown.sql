BEGIN TRANSACTION

	ALTER TABLE [dbo].[MembershipUser] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[Folder] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[File] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[Discussion] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[Comment] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[MembershipUsersInRoles] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[Group] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[GroupUser] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[GroupPermissionForRole] NOCHECK CONSTRAINT all
	
	DROP TABLE #autoUsers 
	SELECT [Id] INTO #autoUsers FROM [dbo].[MembershipUser] WHERE [Email] IN ('autoAdmin@test.co.uk', 'autoGroupAdmin@test.co.uk', 'autoUser@test.co.uk', 'autoUser2@test.co.uk', 'autoUser3@test.co.uk', 'autoTest@email.com', 'VisRegUser@email.com', 'autoEditUser@test.co.uk');
	
    DELETE FROM [dbo].[MembershipUser]
    WHERE [Id] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[File] 
    WHERE [CreatedBy] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[Folder] 
    WHERE [CreatedBy] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [GroupPermissionForRole]
	WHERE [Group_Id] IN (
	    SELECT [Id]
	    FROM [Group] 
	    WHERE [MembershipUser_Id] IN (SELECT * FROM #autoUsers)
    );

    DELETE FROM [dbo].[Group] 
    WHERE [MembershipUser_Id] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[GroupUser] 
    WHERE [MembershipUser_Id] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[MembershipUsersInRoles]
    WHERE [UserIdentifier] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[Comment]
    WHERE [CreatedBy] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[Discussion]
    WHERE [CreatedBy] IN (
        SELECT * FROM #autoUsers
    );

    -- Delete posts made as part of forumSubmission.feature
    DECLARE @forumSubmissionTopic AS uniqueidentifier;
    SELECT @forumSubmissionTopic = Entity_Id FROM [dbo].[Discussion] WHERE Title = 'forumSubmission Discussion';
    DELETE [dbo].[Comment] WHERE [dbo].[Comment].Content LIKE '%Comment posted by the automation%' AND Parent_EntityId = @forumSubmissionTopic;
    DELETE [dbo].[Comment] WHERE [dbo].[Comment].Content LIKE '%This is a reply%' AND Parent_EntityId = @forumSubmissionTopic;
    DELETE [dbo].[Comment] WHERE [dbo].[Comment].Content LIKE '%This is another reply%' AND Parent_EntityId = @forumSubmissionTopic;

    -- Delete Folder generated as part of filesManagement.feature
    DECLARE @adminGroup AS uniqueidentifier;
    SELECT @adminGroup = Id FROM [dbo].[Group] WHERE Name = 'Automation Admin Group'
    DELETE FROM [dbo].[Folder] WHERE Group_Id = @adminGroup AND Title != 'Empty Folder' AND Title != 'Automation Test Folder'
    DELETE FROM [dbo].[Folder] WHERE Group_Id = @adminGroup AND IsDeleted = 1

    -- Delete files uploaded as part of filesNavigation.feature
    DECLARE @rootFolder AS uniqueidentifier;
    SELECT @rootFolder = Id FROM [dbo].[Folder] WHERE Title = 'Empty Folder' AND IsDeleted = 0
    DELETE [dbo].[File] WHERE [ParentFolder] = @rootFolder

    --Delete new user details generated
        -- Invite User Form
    DELETE [dbo].[GroupInvite] WHERE EmailAddress = 'autoTest@email.com'

    --Delete new group generated
        -- Group creation new fields	
    DECLARE @groupid AS uniqueidentifier;
    SELECT @groupid = id FROM [dbo].[Group] WHERE [Name] = 'Automation Created Group';
    DELETE [dbo].[GroupUser] WHERE Group_Id = @groupid
    DELETE [dbo].[Group] WHERE Id = @groupid
	
	ALTER TABLE [dbo].[MembershipUser] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[Folder] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[File] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[Discussion] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[Comment] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[MembershipUsersInRoles] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[Group] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[GroupUser] NOCHECK CONSTRAINT all
	ALTER TABLE [dbo].[GroupPermissionForRole] NOCHECK CONSTRAINT all

	DROP TABLE #autoUsers

ROLLBACK TRANSACTION