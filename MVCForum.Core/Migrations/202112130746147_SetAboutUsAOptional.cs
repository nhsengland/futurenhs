namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetAboutUsAOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Group", "AboutUs", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Group", "AboutUs", c => c.String(nullable: false));
        }
    }
}
