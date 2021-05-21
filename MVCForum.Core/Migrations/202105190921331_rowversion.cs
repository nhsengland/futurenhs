namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rowversion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Group", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Group", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Group", "RowVersion");
            DropColumn("dbo.Group", "IsDeleted");
        }
    }
}
