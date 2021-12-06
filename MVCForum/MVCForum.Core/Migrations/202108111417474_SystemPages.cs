namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class SystemPages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemPage",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    //Set the max length for the column to 50 characters
                    Slug = c.String(nullable: false, maxLength: 50),
                    //Set the max length for the column to 100 characters
                    Title = c.String(nullable: false, maxLength: 100),
                    Content = c.String(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.SystemPage");
        }
    }
}
