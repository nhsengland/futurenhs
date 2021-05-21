namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyGroupsToBeJoinable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        ExtendedDataString = c.String(),
                        Group_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Group", t => t.Group_Id)
                .Index(t => t.Group_Id);
            
            CreateTable(
                "dbo.GroupUser",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Approved = c.Boolean(nullable: false),
                        Locked = c.Boolean(nullable: false),
                        Banned = c.Boolean(nullable: false),
                        RequestToJoinDate = c.DateTime(nullable: false),
                        ApprovedToJoinDate = c.DateTime(),
                        RequestToJoinReason = c.String(),
                        LockReason = c.String(),
                        BanReason = c.String(),
                        ApprovingMembershipUser_Id = c.Guid(),
                        Group_Id = c.Guid(nullable: false),
                        MembershipRole_Id = c.Guid(nullable: false),
                        MembershipUser_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MembershipUser", t => t.ApprovingMembershipUser_Id)
                .ForeignKey("dbo.Group", t => t.Group_Id, cascadeDelete: true)
                .ForeignKey("dbo.MembershipRole", t => t.MembershipRole_Id)
                .ForeignKey("dbo.MembershipUser", t => t.MembershipUser_Id, cascadeDelete: true)
                .Index(t => t.ApprovingMembershipUser_Id)
                .Index(t => t.Group_Id)
                .Index(t => t.MembershipRole_Id)
                .Index(t => t.MembershipUser_Id);
            
            AddColumn("dbo.Topic", "Category_Id", c => c.Guid());
            CreateIndex("dbo.Topic", "Category_Id");
            AddForeignKey("dbo.Topic", "Category_Id", "dbo.Category", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupUser", "MembershipUser_Id", "dbo.MembershipUser");
            DropForeignKey("dbo.GroupUser", "MembershipRole_Id", "dbo.MembershipRole");
            DropForeignKey("dbo.GroupUser", "Group_Id", "dbo.Group");
            DropForeignKey("dbo.GroupUser", "ApprovingMembershipUser_Id", "dbo.MembershipUser");
            DropForeignKey("dbo.Topic", "Category_Id", "dbo.Category");
            DropForeignKey("dbo.Category", "Group_Id", "dbo.Group");
            DropIndex("dbo.GroupUser", new[] { "MembershipUser_Id" });
            DropIndex("dbo.GroupUser", new[] { "MembershipRole_Id" });
            DropIndex("dbo.GroupUser", new[] { "Group_Id" });
            DropIndex("dbo.GroupUser", new[] { "ApprovingMembershipUser_Id" });
            DropIndex("dbo.Category", new[] { "Group_Id" });
            DropIndex("dbo.Topic", new[] { "Category_Id" });
            DropColumn("dbo.Topic", "Category_Id");
            DropTable("dbo.GroupUser");
            DropTable("dbo.Category");
        }
    }
}
