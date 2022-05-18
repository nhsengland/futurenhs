BEGIN TRANSACTION 

    -- Disable constraints for all tables:
    EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all';

	DECLARE @autoAdmin AS uniqueidentifier;
	SELECT @autoAdmin = id FROM [dbo].[MembershipUser] WHERE Email = 'autoAdmin@test.co.uk';
	DECLARE @groupAdmin AS uniqueidentifier;
	SELECT @groupAdmin = id FROM [dbo].[MembershipUser] WHERE Email = 'autoGroupAdmin@test.co.uk';
	DECLARE @autoUser AS uniqueidentifier;
	SELECT @autoUser = id FROM [dbo].[MembershipUser] WHERE Email = 'autoUser@test.co.uk';
	DECLARE @autoUser2 AS uniqueidentifier;
	SELECT @autoUser2 = id FROM [dbo].[MembershipUser] WHERE Email = 'autoUser2@test.co.uk';
	DECLARE @autoUser3 AS uniqueidentifier;
	SELECT @autoUser3 = id FROM [dbo].[MembershipUser] WHERE Email = 'autoUser3@test.co.uk';

	-- Table of all entityIds created by data submitted through the automation to be deleted
	DECLARE @entityTable TABLE (id VARCHAR(50));

	-- Delete posts made as part of forumSubmission.feature
	DECLARE @forumSubmissionTopic AS uniqueidentifier;
	SELECT @forumSubmissionTopic = Entity_Id FROM [dbo].[Discussion] WHERE Title = 'forumSubmission Discussion';
	INSERT INTO @entityTable SELECT Entity_Id FROM [dbo].[Comment] WHERE Content LIKE '%Comment posted by the automation%' AND Parent_EntityId = @forumSubmissionTopic;
	INSERT INTO @entityTable SELECT Entity_Id FROM [dbo].[Comment] WHERE Content LIKE '%This is a reply%' AND Parent_EntityId = @forumSubmissionTopic;
	INSERT INTO @entityTable SELECT Entity_Id FROM [dbo].[Comment] WHERE Content LIKE '%This is another reply%' AND Parent_EntityId = @forumSubmissionTopic;
	DELETE [dbo].[Comment] WHERE [dbo].[Comment].Content LIKE '%Comment posted by the automation%' AND Parent_EntityId = @forumSubmissionTopic;
	DELETE [dbo].[Comment] WHERE [dbo].[Comment].Content LIKE '%This is a reply%' AND Parent_EntityId = @forumSubmissionTopic;
	DELETE [dbo].[Comment] WHERE [dbo].[Comment].Content LIKE '%This is another reply%' AND Parent_EntityId = @forumSubmissionTopic;

	-- Delete discussions generated as part of forumAdmin.feature
	DECLARE @automationDiscussions AS uniqueidentifier;
	SELECT @automationDiscussions = Entity_Id FROM [dbo].[Discussion] WHERE CreatedBy = @groupAdmin;
	INSERT INTO @entityTable SELECT Entity_Id FROM [dbo].[Comment] WHERE CreatedBy = @groupAdmin;
	INSERT INTO @entityTable SELECT Entity_Id FROM [dbo].[Discussion] WHERE CreatedBy = @groupAdmin;
	DELETE [dbo].[Comment] WHERE Parent_EntityId = @automationDiscussions;
	DELETE [dbo].[Discussion] WHERE CreatedBy = @groupAdmin;

	--Delete images that have been uploaded
	DELETE FROM [dbo].[Image]
	WHERE [Id] IN (
		(SELECT ImageId FROM [MembershipUser] WHERE email = 'autoEditUser@test.co.uk'),
		(SELECT ImageId FROM [Group] WHERE Name = 'Automation Edited Group')
	)

	-- Delete Folder generated as part of filesManagement.feature
	DECLARE @adminGroup AS uniqueidentifier;
	SELECT @adminGroup = Id FROM [dbo].[Group] WHERE Name = 'Automation Admin Group';
	DELETE FROM [dbo].[Folder] WHERE Group_Id = @adminGroup AND CreatedBy = @groupAdmin;
	DELETE FROM [dbo].[Folder] WHERE Group_Id = @adminGroup AND IsDeleted = 1;

	-- Delete files uploaded as part of filesNavigation.feature
	DECLARE @rootFolder AS uniqueidentifier;
	SELECT @rootFolder = Id FROM [dbo].[Folder] WHERE title = 'Empty Folder' AND IsDeleted = 0;
	DELETE [dbo].[File] WHERE [ParentFolder] = @rootFolder;

	-- Reset Users to Pending Group Access groupAdmin.feature
		-- Approve group member request
		-- Reject group member request
	UPDATE [dbo].[GroupUser] SET Approved = 0, ApprovedToJoinDateUTC = NULL, ApprovingMembershipUser_Id = NULL WHERE MembershipUser_Id = @autoUser2 AND Group_Id = @adminGroup;
	UPDATE [dbo].[GroupUser] SET Rejected = 0, ApprovedToJoinDateUTC = NULL, ApprovingMembershipUser_Id = NULL WHERE MembershipUser_Id = @autoUser3 AND Group_Id = @adminGroup;

	-- Remove users from groups added as part of automation
		-- Add registered user
	DECLARE @publicGroup AS uniqueidentifier;
	SELECT @publicGroup = Id FROM [dbo].[Group] WHERE Name = 'Automation Public Group';
	DELETE FROM [dbo].[GroupUser] WHERE MembershipUser_Id = @autoUser2 AND Group_Id = @publicGroup;

		-- Join a private group
	DECLARE @privateGroup AS uniqueidentifier;
	SELECT @privateGroup = Id FROM [dbo].[Group] WHERE Name = 'Automation Private Group';
	DELETE FROM [dbo].[GroupUser] WHERE MembershipUser_Id = @autoUser AND Group_Id = @privateGroup;

	--Delete new user details generated
		-- Invite User Form
	DELETE [dbo].[GroupInvite] WHERE EmailAddress = 'autoTest@email.com';
		-- New user registration
	DELETE [dbo].[MembershipUser] WHERE UserName = 'autoTest@email.com';

	--Delete new group generated
		-- Group creation new fields	
	DECLARE @groupid AS uniqueidentifier;
	SELECT @groupid = id FROM [dbo].[Group] WHERE [Name] = 'Automation Created Group';

	DELETE [dbo].[GroupUser] WHERE Group_Id = @groupid;
	DELETE [dbo].[Group] WHERE Id = @groupid;

	-- Reset changes made to Automation Editable Group information
	DECLARE @editgroup AS uniqueidentifier;
	SELECT @editgroup = id FROM [dbo].[Group] WHERE [Name] = 'Automation Edited Group';
	UPDATE [dbo].[Group] SET 
		Name = 'Automation Editable Group',
		Description = 'DO NOT USE - This group is reserved solely for use by our automated test scripts', 
		Subtitle = 'DO NOT USE - This group is reserved solely for use by our automated test scripts', 
		PublicGroup = 0, 
		Introduction = 'Introduction ',
		ImageId = NULL,
		AboutUs = NULL
	WHERE Id = @editgroup;

	-- Reset changes made to autoEditUser in edit profile tests
	DECLARE @editUser AS uniqueidentifier;
	SELECT @editUser = id FROM [dbo].[MembershipUser] WHERE Email = 'autoEditUser@test.co.uk';
	UPDATE [dbo].[MembershipUser] SET 
		FirstName = 'autoEdit', 
		Surname = 'User',
		Initials = 'AU',
		ImageId = NULL,
		Pronouns = ''
	WHERE Id = @editUser;

	-- Reset changes made to docTest file in Automation Test Folder
	DECLARE @docTest AS uniqueidentifier;
	SELECT @docTest = id FROM [dbo].[File] WHERE Title = 'Doc Test';
	UPDATE [dbo].[File] SET Title = 'docTest', Description = 'test doc' WHERE Id = @docTest;

	-- Reset changes made to DeleteFolder 
	DECLARE @deletefolder AS uniqueidentifier;
	SELECT @deletefolder = id FROM [dbo].[Folder] WHERE Title = 'DeleteFolder';
	UPDATE [dbo].[Folder] SET IsDeleted = 0 WHERE Id = @deletefolder;

	-- Reset changes made to EditableFolder 
	DECLARE @editfolder AS uniqueidentifier;
	SELECT @editfolder = id FROM [dbo].[Folder] WHERE Title = 'EditedFolder';
	UPDATE [dbo].[Folder] SET Title = 'EditableFolder' WHERE Id = @editfolder;
	
	-- Delete all values in Entity Table from submitted data
	DELETE FROM [dbo].[Entity] WHERE [Id] IN (SELECT * FROM @entityTable);

    -- Re-enable constraints for all tables:
    EXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all';	

ROLLBACK TRANSACTION