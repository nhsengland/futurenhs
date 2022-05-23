BEGIN

	-- Disable constraints for all tables:
    EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'

    SELECT [Id] INTO #autoUsers FROM [dbo].[MembershipUser] WHERE [Email] IN ('autoAdmin@test.co.uk', 'autoGroupAdmin@test.co.uk', 'autoUser@test.co.uk', 'autoUser2@test.co.uk', 'autoUser3@test.co.uk', 'autoTest@email.com', 'VisRegUser@email.com', 'autoEditUser@test.co.uk');
	DECLARE @groupAdmin AS uniqueidentifier;
    SELECT @groupAdmin = id FROM [dbo].[MembershipUser] WHERE Email = 'autoGroupAdmin@test.co.uk';
    DECLARE @autoUser AS uniqueidentifier;
    SELECT @autoUser = id FROM [dbo].[MembershipUser] WHERE Email = 'autoUser@test.co.uk';

	DECLARE @entityTable TABLE (id VARCHAR(50))
	INSERT INTO @entityTable SELECT Entity_Id FROM Comment WHERE Content ='<p>First Comment</p>' AND Parent_EntityId = '83702899-fdbf-49ed-bb1d-adda00f757c3'
	INSERT INTO @entityTable SELECT Entity_Id FROM Comment WHERE Content ='<p>Third Comment</p>' AND Parent_EntityId = '83702899-fdbf-49ed-bb1d-adda00f757c3'
	INSERT INTO @entityTable SELECT Entity_Id FROM Comment WHERE Content ='<p>Fourth Comment</p>' AND Parent_EntityId = '83702899-fdbf-49ed-bb1d-adda00f757c3'
	INSERT INTO @entityTable SELECT Entity_Id FROM Comment WHERE Content ='<p>Fifth Comment</p>' AND Parent_EntityId = '83702899-fdbf-49ed-bb1d-adda00f757c3'
	INSERT INTO @entityTable SELECT Entity_Id FROM Comment WHERE Content ='<p>Sixth Comment</p>' AND Parent_EntityId = '83702899-fdbf-49ed-bb1d-adda00f757c3'
	INSERT INTO @entityTable SELECT Entity_Id FROM Comment WHERE Content ='<p>Seventh Comment</p>' AND Parent_EntityId = '83702899-fdbf-49ed-bb1d-adda00f757c3'
	INSERT INTO @entityTable SELECT Entity_Id FROM Comment WHERE Content ='<p>Eighth Comment</p>' AND Parent_EntityId = '83702899-fdbf-49ed-bb1d-adda00f757c3'
	INSERT INTO @entityTable SELECT Entity_Id FROM Comment WHERE Content ='<p>Ninth Comment</p>' AND Parent_EntityId = '83702899-fdbf-49ed-bb1d-adda00f757c3'
	INSERT INTO @entityTable SELECT Entity_Id FROM Comment WHERE Content ='<p>Tenth Comment</p>' AND Parent_EntityId = '83702899-fdbf-49ed-bb1d-adda00f757c3'
	INSERT INTO @entityTable SELECT Entity_Id FROM Comment WHERE Content ='<p>Eleventh Comment</p>' AND Parent_EntityId = '83702899-fdbf-49ed-bb1d-adda00f757c3'
	INSERT INTO @entityTable VALUES (N'ca5c5162-7a2a-482e-b15b-ad7f010e30cf'), (N'edcc5437-b704-45d8-8972-ad7f010e2316'), (N'e56abf3c-7aa0-465a-a146-ad7f010e4acc'), (N'bcca8784-0e5e-496c-9bbb-ad7f010e51b4'), (N'e846a7bc-327a-4c05-89c7-ad7f010e5ed2'), (N'e4940ced-087d-41e9-a350-ad7f010e6bc9'), (N'69f7c324-3c63-4917-8b22-ad7f010e719e'), (N'58ab24e5-f3f9-4f8d-aba9-ad7f010e809d'), (N'107d1f85-3366-410c-8ffe-ad7f010e8586'), (N'0e807603-6d32-4f4f-833b-ad7f010e903a'), (N'e28ed14b-46ae-4fdd-8988-ad7f010e959c'), (N'80d3fc63-4852-4163-98ac-ad880118903c'), 
	(N'5313414a-ac9d-49eb-af90-ad880118b1ca'), (N'b578b2e5-cce4-4e84-9e64-ad880118bce2'), (N'c30117fc-dbb5-453c-9968-ad880118c70e'), (N'1af57d07-9012-4511-b9fc-ad88012033b0'), (N'4760e89f-048e-4b47-a70e-339cd43a2703'), (N'3cb17bfa-9ad4-4ab7-9801-36aff0278be0'), (N'5ca2c29e-31d6-448e-a409-6fbf83cbf844'), (N'9652f206-bd13-4920-b6d4-76858a21029a'), (N'13fa753a-fcba-4bf3-ae4c-771103503246'), (N'c0011070-c541-450e-a19f-7b4f91640835'), (N'285a2aa3-bfb8-41e1-b4d1-9349a68f2252'), (N'ce818d68-d4c5-4518-8b4f-982930b5f8c1'), (N'7120144f-a4a3-40dc-bd83-b7f8e7f8f66d'), 
	(N'468cf575-9dcd-4897-9976-f747009dc13d'), (N'4654a062-b693-42f5-9b25-c8a286fa8ba9'), (N'493a2f0e-4195-4730-9f3c-ad7f010d40c8'), (N'3fbc0d4e-c2a7-46a0-99e9-ad7f010eb4ad'), (N'a4bf9b51-d145-4601-92b6-ad7f010ebe84'), (N'6ea66f95-24bc-4179-94f4-ad7f010ec730'), (N'a6da1550-066f-4ae6-ae98-ad7f010ece99'), (N'e1d477d3-dfb0-4acd-b74d-ad7f010ed722'), (N'55f516e0-9117-4a43-9363-ad8200ce7d1a'), (N'96b155ba-e8de-40e3-980e-ad8800b0a6b8'), (N'83702899-fdbf-49ed-bb1d-adda00f757c3'), (N'5889c069-5061-403b-93f6-adda00f7638a'), (N'f91c0e75-9d3d-4700-9efb-adda00f770d0'), (N'eaf7a8f1-b9a0-415d-b26d-88517132536e')
    INSERT INTO @entityTable SELECT Entity_Id FROM [dbo].[Comment] WHERE CreatedBy = @groupAdmin;
    INSERT INTO @entityTable SELECT Entity_Id FROM [dbo].[Comment] WHERE CreatedBy = @autoUser;
    INSERT INTO @entityTable SELECT Entity_Id FROM [dbo].[Discussion] WHERE CreatedBy = @groupAdmin;
	
	DELETE FROM [dbo].[Group] 
    WHERE [GroupOwner] IN (
        SELECT * FROM #autoUsers
    );

	DELETE FROM [dbo].[Image]
	WHERE [CreatedBy] IN (
		 SELECT [Id] FROM #autoUsers
	)

	DELETE FROM [dbo].[Entity] 
    WHERE [Id] IN (
        SELECT * FROM @entityTable
    );

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

    --DELETE FROM [GroupPermissionForRole]
	--WHERE [Group_Id] IN (
	    --SELECT [Id]
	    --FROM [Group] 
	    --WHERE [MembershipUser_Id] IN (SELECT * FROM #autoUsers)
    --);

    DELETE FROM [dbo].[GroupUser] 
    WHERE [MembershipUser_Id] IN (
        SELECT * FROM #autoUsers
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
	
	DROP TABLE #autoUsers

    -- Re-enable constraints for all tables:
    EXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all';	

END