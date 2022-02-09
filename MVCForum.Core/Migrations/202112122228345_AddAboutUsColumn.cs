namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAboutUsColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Group", "AboutUs", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Group", "AboutUs");
        }
    }
}
