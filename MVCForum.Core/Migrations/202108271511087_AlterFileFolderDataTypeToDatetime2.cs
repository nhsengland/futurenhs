namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterFileFolderDataTypeToDatetime2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.File", "CreatedAtUtc", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.File", "ModifiedAtUtc", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Folder", "CreatedAtUtc", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Folder", "CreatedAtUtc", c => c.DateTime(nullable: false));
            AlterColumn("dbo.File", "ModifiedAtUtc", c => c.DateTime());
            AlterColumn("dbo.File", "CreatedAtUtc", c => c.DateTime(nullable: false));
        }
    }
}
