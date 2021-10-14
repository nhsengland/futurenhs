namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteFolderStoredProcedure : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "usp_delete_folder",
                p => new
                {
                    FolderId = p.Guid()
                },
                body:
					@"
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
					END CATCH
				
					SELECT @success;
                "
			);
        }
        
        public override void Down()
        {
            DropStoredProcedure("usp_delete_folder");
        }
    }
}
