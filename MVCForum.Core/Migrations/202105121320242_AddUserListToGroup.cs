namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserListToGroup : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GroupUser", "Group_Id", "dbo.Group");
            AddForeignKey("dbo.GroupUser", "Group_Id", "dbo.Group", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupUser", "Group_Id", "dbo.Group");
            AddForeignKey("dbo.GroupUser", "Group_Id", "dbo.Group", "Id", cascadeDelete: true);
        }
    }
}
