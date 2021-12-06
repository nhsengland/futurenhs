namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameDateColumnOnFileAndFolderTable : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.File", "CreatedDate", "CreatedAtUtc");
            RenameColumn("dbo.File", "ModifiedDate", "ModifiedAtUtc");
            RenameColumn("dbo.Folder", "DateAdded", "CreatedAtUtc");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Folder", "CreatedAtUtc", "CreatedDate");
            RenameColumn("dbo.File", "ModifiedAtUtc", "ModifiedDate");
            RenameColumn("dbo.File", "CreatedAtUtc", "DateAdded");
        }
    }
}
