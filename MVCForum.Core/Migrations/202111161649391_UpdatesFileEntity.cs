namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatesFileEntity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.File", "Title", c => c.String(nullable: false, maxLength: 1000));
            AlterColumn("dbo.File", "Description", c => c.String(nullable: true, maxLength: 4000));
            AlterColumn("dbo.File", "FileName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.File", "BlobName", c => c.String(nullable: true, maxLength: 42));
            DropForeignKey("dbo.File", "FK_dbo.File_dbo.UploadStatus_UploadStatus");
            AlterColumn("dbo.File", "FileStatus", c => c.Int(nullable: false));
            AddForeignKey("dbo.File", "FileStatus", "FileStatus", "Id", false, "FK_dbo.File_dbo.UploadStatus_UploadStatus");
        }

        public override void Down()
        {
            AlterColumn("dbo.File", "Title", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.File", "Description", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.File", "FileName", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.File", "BlobName", c => c.String(nullable: true, maxLength: 100));
            DropForeignKey("dbo.File", "FK_dbo.File_dbo.UploadStatus_UploadStatus");
            AlterColumn("dbo.File", "FileStatus", c => c.Int(nullable: true));
            AddForeignKey("dbo.File", "FileStatus", "FileStatus", "Id", false, "FK_dbo.File_dbo.UploadStatus_UploadStatus");
        }
    }
}
