namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifiesFileProperties : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.UploadStatus", newName: "FileStatus");
            RenameColumn("dbo.File", "FileSize", "FileSizeBytes");
            RenameColumn("dbo.File", "UploadStatus", "FileStatus");
            RenameColumn("dbo.File", "FileUrl", "BlobName");
            AlterColumn("dbo.File", "BlobName", c => c.String(nullable: true, maxLength: 100));
            AlterColumn("dbo.File", "Title", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.File", "Description", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.File", "FileName", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.File", "FileExtension", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.File", "FileExtension", c => c.String());
            AlterColumn("dbo.File", "FileName", c => c.String());
            AlterColumn("dbo.File", "Description", c => c.String());
            AlterColumn("dbo.File", "Title", c => c.String());
            AlterColumn("dbo.File", "BlobName", c => c.String());
            RenameColumn("dbo.File", "FileSizeBytes", "FileSize");
            RenameColumn("dbo.File", "FileStatus", "UploadStatus");
            RenameColumn("dbo.File", "BlobName", "FileUrl");

            RenameTable(name: "dbo.FileStatus", newName: "UploadStatus");
        }
    }
}
