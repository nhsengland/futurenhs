namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddsFilesEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.File",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        FileName = c.String(),
                        FileSize = c.String(),
                        FileExtension = c.String(),
                        FileUrl = c.String(),
                        CreatedBy = c.Guid(nullable: false),
                        ModifiedBy = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        ParentFolder_FolderId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Folder", t => t.ParentFolder_FolderId)
                .Index(t => t.ParentFolder_FolderId);
            
            CreateTable(
                "dbo.Folder",
                c => new
                    {
                        FolderId = c.Guid(nullable: false),
                        FolderName = c.String(nullable: false, maxLength: 512),
                        Description = c.String(),
                        AddedBy = c.Guid(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                        ParentId = c.Guid(),
                        Status = c.Int(nullable: false),
                        Folder2_FolderId = c.Guid(),
                    })
                .PrimaryKey(t => t.FolderId)
                .ForeignKey("dbo.Folder", t => t.Folder2_FolderId)
                .Index(t => t.Folder2_FolderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.File", "ParentFolder_FolderId", "dbo.Folder");
            DropForeignKey("dbo.Folder", "Folder2_FolderId", "dbo.Folder");
            DropIndex("dbo.Folder", new[] { "Folder2_FolderId" });
            DropIndex("dbo.File", new[] { "ParentFolder_FolderId" });
            DropTable("dbo.Folder");
            DropTable("dbo.File");
        }
    }
}
