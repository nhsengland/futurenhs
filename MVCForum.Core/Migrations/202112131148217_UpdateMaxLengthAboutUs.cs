namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMaxLengthAboutUs : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Group", "AboutUs", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Group", "AboutUs", c => c.String());
        }
    }
}
