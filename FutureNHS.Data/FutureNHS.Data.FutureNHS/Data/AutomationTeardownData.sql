
IF EXISTS (select id from [MembershipUser] where [UserName] = 'autoAdmin@test.co.uk')
BEGIN
    -- Disable constraints for all tables:
    EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'

    SELECT [Id] INTO #autoUsers FROM [dbo].[MembershipUser] WHERE [Email] IN ('autoAdmin@test.co.uk', 'autoUser@test.co.uk', 'autoUser2@test.co.uk', 'autoUser3@test.co.uk', 'autoTest@email.com', 'VisRegUser@email.com', 'autoEditUser@test.co.uk');

    DELETE FROM [dbo].[MembershipUser]
    WHERE [Id] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[File] 
    WHERE [CreatedBy] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[Folder] 
    WHERE [AddedBy] IN (
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

    DELETE FROM [dbo].[MembershipUserPoints] 
    WHERE [MembershipUser_Id] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[MembershipUsersInRoles]
    WHERE [UserIdentifier] IN (
        SELECT * FROM #autoUsers
    );

    

    DELETE FROM [dbo].[SystemPage]
    WHERE [Slug] = 'repeat-slug-test';

    DELETE FROM [dbo].[Post]
    WHERE [MembershipUser_Id] IN (
        SELECT * FROM #autoUsers
    );

    DELETE FROM [dbo].[Topic]
    WHERE [MembershipUser_Id] IN (
        SELECT * FROM #autoUsers
    );

    -- Delete posts made as part of forumSubmission.feature
    DECLARE @rootPost AS uniqueidentifier;
    SELECT @rootPost = id FROM [dbo].[Post] WHERE PostContent = '<p>Discussion for scenarios for forumNavigation and forumSubmission features</p>';
    DECLARE @forumSubmissionTopic AS uniqueidentifier;
    SELECT @forumSubmissionTopic = id FROM [dbo].[Topic] WHERE Name = 'forumSubmission Discussion';
    DELETE [dbo].[Post] WHERE [dbo].[Post].PostContent LIKE '%Comment posted by the automation%' AND Topic_Id = @forumSubmissionTopic;
    DELETE [dbo].[Post] WHERE [dbo].[Post].PostContent LIKE '%This is a reply%' AND Topic_Id = @forumSubmissionTopic;
    DELETE [dbo].[Post] WHERE [dbo].[Post].PostContent LIKE '%This is another reply%' AND Topic_Id = @forumSubmissionTopic;

    -- Delete Folder generated as part of filesManagement.feature
    DECLARE @adminGroup AS uniqueidentifier;
    SELECT @adminGroup = Id FROM [dbo].[Group] WHERE Name = 'Automation Admin Group'
    DELETE FROM [dbo].[Folder] WHERE ParentGroup = @adminGroup AND Name != 'Empty Folder' AND Name != 'Automation Test Folder'
    DELETE FROM [dbo].[Folder] WHERE ParentGroup = @adminGroup AND IsDeleted = 1

    -- Delete files uploaded as part of filesNavigation.feature
    DECLARE @rootFolder AS uniqueidentifier;
    SELECT @rootFolder = Id FROM [dbo].[Folder] WHERE Name = 'Empty Folder' AND IsDeleted = 0

    DELETE [dbo].[File] WHERE [ParentFolder] = @rootFolder

    -- Remove deleted System Page from database generated by siteAdmin.feature
    DELETE [dbo].[SystemPage] WHERE Slug = 'automation-content-page'
    DELETE [dbo].[SystemPage] WHERE Slug = 'editable-system-page'
    DELETE [dbo].[SystemPage] WHERE Slug = 'deletable-system-page'

    --Delete new user details generated
        -- Invite User Form
    DELETE [dbo].[GroupInvite] WHERE EmailAddress = 'autoTest@email.com'

    --Delete new group generated
        -- Group creation new fields	
    DECLARE @groupid AS uniqueidentifier;
    SELECT @groupid = id FROM [dbo].[Group] WHERE [Name] = 'alphaGroupTest';

    DELETE [dbo].[GroupUser] WHERE Group_Id = @groupid
    DELETE [dbo].[Group] WHERE Id = @groupid

    -- Re-enable constraints for all tables:
    EXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all';	
    
END