CREATE PROCEDURE [usp_delete_folder]
    @FolderId [uniqueidentifier]
AS
BEGIN
    
    					BEGIN TRANSACTION;
    					DECLARE @success BIT;
    					SET @success = 0;
    					BEGIN TRY
    						WITH cte_Folders AS
    						(
    							SELECT	Id
    							FROM	Folder 
    							WHERE	Id = @FolderId
    
    							UNION ALL
    
    							SELECT		c.Id
    							FROM		Folder c
    							INNER JOIN	cte_Folders f
    								ON		f.Id = c.ParentFolder
    						)
    
    						-- Add folder data to temp table so can use to get file data for folders
    						SELECT	Id 
    						INTO	#Folders
    						FROM	cte_Folders
    
    						-- Update folders to mark them as deleted
    						UPDATE Folder 
    						SET IsDeleted = 1 
    						WHERE Id IN (SELECT Id FROM #Folders);
    
    						-- Update files to mark them as deleted
    						UPDATE [File]  
    						SET FileStatus = 7
    						WHERE	ParentFolder IN 
    										(
    											SELECT	Id 
    											FROM	#Folders
    										)
    
    						-- Drop the temp table
    						DROP TABLE #Folders;
    						COMMIT TRANSACTION;   
    						SET @success = 1;
    					END TRY
    					BEGIN CATCH
    						-- rollback transaction if there is an exception
    						ROLLBACK TRANSACTION; 
    						THROW;
    					END CATCH
    				
    					SELECT @success;
    
END