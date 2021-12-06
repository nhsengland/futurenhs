namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FoldersAndFiles : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Folder", "Folder2_FolderId", "dbo.Folder");
            DropForeignKey("dbo.File", "ParentFolder_FolderId", "dbo.Folder");
            DropIndex("dbo.File", new[] { "ParentFolder_FolderId" });
            DropIndex("dbo.Folder", new[] { "Folder2_FolderId" });
            DropPrimaryKey("dbo.Folder");
            AddColumn("dbo.File", "ParentFolder", c => c.Guid(nullable: false));
            AddColumn("dbo.Folder", "Id", c => c.Guid(nullable: false));
            AddColumn("dbo.Folder", "Name", c => c.String(nullable: false, maxLength: 450));
            AddColumn("dbo.Folder", "ParentFolder", c => c.Guid());
            AddColumn("dbo.Folder", "FileCount", c => c.Int(nullable: false));
            AddColumn("dbo.Folder", "ParentGroup", c => c.Guid(nullable: false));
            AddColumn("dbo.Folder", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Folder", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AlterColumn("dbo.File", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.File", "FileName", c => c.String(nullable: false));
            AlterColumn("dbo.File", "FileSize", c => c.String(nullable: false));
            AlterColumn("dbo.File", "FileExtension", c => c.String(nullable: false));
            AlterColumn("dbo.File", "FileUrl", c => c.String(nullable: false));
            AddPrimaryKey("dbo.Folder", "Id");
            DropColumn("dbo.File", "ParentFolder_FolderId");
            DropColumn("dbo.Folder", "FolderId");
            DropColumn("dbo.Folder", "FolderName");
            DropColumn("dbo.Folder", "ParentId");
            DropColumn("dbo.Folder", "Status");
            DropColumn("dbo.Folder", "Folder2_FolderId");

            AddForeignKey("dbo.Folder", "ParentFolder", "dbo.Folder", "Id", cascadeDelete: false);
            CreateIndex("dbo.Folder", "ParentFolder");
            AddForeignKey("dbo.Folder", "AddedBy", "dbo.MembershipUser", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Folder", "ParentGroup", "dbo.Group", "Id", cascadeDelete: false);
            CreateIndex("dbo.Folder", "ParentGroup");
            AddForeignKey("dbo.File", "ParentFolder", "dbo.Folder", "Id", cascadeDelete: false);
            CreateIndex("dbo.File", "ParentFolder");
            AddForeignKey("dbo.File", "CreatedBy", "dbo.MembershipUser", "Id", cascadeDelete: false);
            AddForeignKey("dbo.File", "ModifiedBy", "dbo.MembershipUser", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            AddColumn("dbo.Folder", "Folder2_FolderId", c => c.Guid());
            AddColumn("dbo.Folder", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Folder", "ParentId", c => c.Guid());
            AddColumn("dbo.Folder", "FolderName", c => c.String(nullable: false, maxLength: 512));
            AddColumn("dbo.Folder", "FolderId", c => c.Guid(nullable: false));
            AddColumn("dbo.File", "ParentFolder_FolderId", c => c.Guid());
            DropPrimaryKey("dbo.Folder");
            AlterColumn("dbo.File", "FileUrl", c => c.String());
            AlterColumn("dbo.File", "FileExtension", c => c.String());
            AlterColumn("dbo.File", "FileSize", c => c.String());
            AlterColumn("dbo.File", "FileName", c => c.String());
            AlterColumn("dbo.File", "Title", c => c.String());
            DropColumn("dbo.Folder", "RowVersion");
            DropColumn("dbo.Folder", "IsDeleted");
            DropColumn("dbo.Folder", "ParentGroup");
            DropColumn("dbo.Folder", "FileCount");
            DropColumn("dbo.Folder", "ParentFolder");
            DropColumn("dbo.Folder", "Name");
            DropColumn("dbo.Folder", "Id");
            DropColumn("dbo.File", "ParentFolder");
            AddPrimaryKey("dbo.Folder", "FolderId");
            CreateIndex("dbo.Folder", "Folder2_FolderId");
            CreateIndex("dbo.File", "ParentFolder_FolderId");
            AddForeignKey("dbo.File", "ParentFolder_FolderId", "dbo.Folder", "FolderId");
            AddForeignKey("dbo.Folder", "Folder2_FolderId", "dbo.Folder", "FolderId");

            DropForeignKey("dbo.Folder", "ParentFolder", "dbo.Folder");
            DropForeignKey("dbo.Folder", "AddedBy", "dbo.Folder");
            DropForeignKey("dbo.File", "CreatedBy", "dbo.MembershipUser");
            DropForeignKey("dbo.File", "ModifiedBy", "dbo.MembershipUser");
        }
    }
}
