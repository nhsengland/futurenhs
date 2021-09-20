namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class makesFilePropertiesNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.File", "ModifiedBy", c => c.Guid());
            AlterColumn("dbo.File", "ModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.File", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.File", "ModifiedBy", c => c.Guid(nullable: false));
        }
    }
}
