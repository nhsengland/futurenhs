namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Image",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FileName = c.String(nullable: false, maxLength: 45),
                        FileSizeBytes = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        MediaType = c.String(nullable: false, maxLength: 10),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Group", "HeaderImage", c => c.Guid());
            AddForeignKey("dbo.Group", "HeaderImage", "dbo.Image", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Group", "HeaderImage", "dbo.Image");
            DropColumn("dbo.Group", "HeaderImage");
            DropTable("dbo.Image");
        }
    }
}
