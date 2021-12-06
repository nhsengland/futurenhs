namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropIPColumnFromPost : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Post", "IpAddress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Post", "IpAddress", c => c.String(maxLength: 50));
        }
    }
}
