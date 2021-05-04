namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePrivateMessaging : DbMigration
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PrivateMessage", "UserFrom_Id", "dbo.MembershipUser");
            DropForeignKey("dbo.PrivateMessage", "UserTo_Id", "dbo.MembershipUser");
            DropIndex("dbo.PrivateMessage", new[] { "UserFrom_Id" });
            DropIndex("dbo.PrivateMessage", new[] { "UserTo_Id" });
            DropTable("dbo.PrivateMessage");
        }
    }
}
