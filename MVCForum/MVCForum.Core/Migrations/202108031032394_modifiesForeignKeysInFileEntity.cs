namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifiesForeignKeysInFileEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UploadStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.File", "UploadStatus", c => c.Int(nullable: false));
            AddForeignKey("dbo.File", "UploadStatus", "dbo.UploadStatus", "Id");
            AlterColumn("dbo.File", "Title", c => c.String());
            AlterColumn("dbo.File", "FileName", c => c.String());
            AlterColumn("dbo.File", "FileSize", c => c.String());
            AlterColumn("dbo.File", "FileExtension", c => c.String());
            AlterColumn("dbo.File", "FileUrl", c => c.String());
            AlterColumn("dbo.File", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.File", "ModifiedBy", c => c.Guid());
            AlterColumn("dbo.Folder", "Name", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Folder", "Name", c => c.String(nullable: false, maxLength: 450));
            AlterColumn("dbo.File", "FileUrl", c => c.String(nullable: false));
            AlterColumn("dbo.File", "FileExtension", c => c.String(nullable: false));
            AlterColumn("dbo.File", "FileSize", c => c.String(nullable: false));
            AlterColumn("dbo.File", "FileName", c => c.String(nullable: false));
            AlterColumn("dbo.File", "Title", c => c.String(nullable: false));
            DropForeignKey("dbo.File", "UploadStatus", "dbo.UploadStatus");
            DropColumn("dbo.File", "UploadStatus");
            DropTable("dbo.UploadStatus");
        }
    }
}
