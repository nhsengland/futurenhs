namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroupInvite : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupInvite",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmailAddress = c.String(nullable: false, maxLength: 254),
                        IsDeleted = c.Boolean(nullable: false),
                        GroupId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            AddForeignKey("dbo.GroupInvite", "GroupId", "dbo.Group", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupInvite", "GroupId", "dbo.Group");
            DropTable("dbo.GroupInvite");
        }
    }
}
