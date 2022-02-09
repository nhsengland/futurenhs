namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateFileEntity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.File", "Title", c => c.String(nullable: false, maxLength: 45));
            AlterColumn("dbo.File", "Description", c => c.String(nullable: true, maxLength: 150));
        }

        public override void Down()
        {
            AlterColumn("dbo.File", "Title", c => c.String(nullable: false, maxLength: 1000));
            AlterColumn("dbo.File", "Description", c => c.String(nullable: true, maxLength: 4000));
        }
    }
}
