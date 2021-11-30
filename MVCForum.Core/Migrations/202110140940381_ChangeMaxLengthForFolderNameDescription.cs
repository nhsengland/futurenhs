namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeMaxLengthForFolderNameDescription : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Folder", "Name", c => c.String(nullable: false, maxLength: 1000));
            AlterColumn("dbo.Folder", "Description", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Folder", "Description", c => c.String());
            AlterColumn("dbo.Folder", "Name", c => c.String(nullable: false, maxLength: 450));
        }
    }
}
