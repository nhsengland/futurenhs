namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddsSubtitleAndIntroductionToGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Group", "Subtitle", c => c.String(maxLength: 254));
            AddColumn("dbo.Group", "Introduction", c => c.String(nullable: false, maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Group", "Introduction");
            DropColumn("dbo.Group", "Subtitle");
        }
    }
}
