namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixMigrations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MembershipUser_Badge", "MembershipUser_Id", "dbo.MembershipUser");
            DropForeignKey("dbo.MembershipUser_Badge", "Badge_Id", "dbo.Badge");
            DropForeignKey("dbo.BadgeTypeTimeLastChecked", "MembershipUser_Id", "dbo.MembershipUser");
            DropForeignKey("dbo.PrivateMessage", "UserTo_Id", "dbo.MembershipUser");
            DropForeignKey("dbo.PrivateMessage", "UserFrom_Id", "dbo.MembershipUser");
            DropIndex("dbo.BadgeTypeTimeLastChecked", new[] { "MembershipUser_Id" });
            DropIndex("dbo.PrivateMessage", new[] { "UserTo_Id" });
            DropIndex("dbo.PrivateMessage", new[] { "UserFrom_Id" });
            DropIndex("dbo.MembershipUser_Badge", new[] { "MembershipUser_Id" });
            DropIndex("dbo.MembershipUser_Badge", new[] { "Badge_Id" });
            DropTable("dbo.Badge");
            DropTable("dbo.BadgeTypeTimeLastChecked");
            DropTable("dbo.PrivateMessage");
            DropTable("dbo.MembershipUser_Badge");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MembershipUser_Badge",
                c => new
                    {
                        MembershipUser_Id = c.Guid(nullable: false),
                        Badge_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.MembershipUser_Id, t.Badge_Id });
            
            CreateTable(
                "dbo.PrivateMessage",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DateSent = c.DateTime(nullable: false),
                        Message = c.String(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        IsSentMessage = c.Boolean(nullable: false),
                        UserTo_Id = c.Guid(nullable: false),
                        UserFrom_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BadgeTypeTimeLastChecked",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BadgeType = c.String(nullable: false, maxLength: 50),
                        TimeLastChecked = c.DateTime(nullable: false),
                        MembershipUser_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Badge",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Type = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 50),
                        DisplayName = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                        Image = c.String(maxLength: 50),
                        AwardsPoints = c.Int(),
                        ExtendedDataString = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.MembershipUser_Badge", "Badge_Id");
            CreateIndex("dbo.MembershipUser_Badge", "MembershipUser_Id");
            CreateIndex("dbo.PrivateMessage", "UserFrom_Id");
            CreateIndex("dbo.PrivateMessage", "UserTo_Id");
            CreateIndex("dbo.BadgeTypeTimeLastChecked", "MembershipUser_Id");
            AddForeignKey("dbo.PrivateMessage", "UserFrom_Id", "dbo.MembershipUser", "Id");
            AddForeignKey("dbo.PrivateMessage", "UserTo_Id", "dbo.MembershipUser", "Id");
            AddForeignKey("dbo.BadgeTypeTimeLastChecked", "MembershipUser_Id", "dbo.MembershipUser", "Id");
            AddForeignKey("dbo.MembershipUser_Badge", "Badge_Id", "dbo.Badge", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MembershipUser_Badge", "MembershipUser_Id", "dbo.MembershipUser", "Id", cascadeDelete: true);
        }
    }
}
