namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompositeGroupUserKey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.GroupUser");
            AddPrimaryKey("dbo.GroupUser", new [] { "MembershipUser_Id", "Group_Id" }, "PK_dbo.GroupUser_MembershipUser_Id_Group_Id", true);
            DropIndex("dbo.GroupUser", "IX_Group_Id");
            CreateIndex(table: "dbo.GroupUser", column: "Id", unique: true,  name: "IX_Group_Id");
        }
        
        public override void Down()
        {
            
            DropPrimaryKey("dbo.GroupUser", "PK_dbo.GroupUser_MembershipUser_Id_Group_Id");
            AddPrimaryKey("dbo.GroupUser","Id");

        }
    }
}
