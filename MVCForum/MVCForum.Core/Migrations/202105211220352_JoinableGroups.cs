namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JoinableGroups : DbMigration
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
                        Rejected = c.Boolean(nullable: false),
                        Locked = c.Boolean(nullable: false),
                        Banned = c.Boolean(nullable: false),
                        RequestToJoinDate = c.DateTime(nullable: false),
                        ApprovedToJoinDate = c.DateTime(),
                        RequestToJoinReason = c.String(),
                        LockReason = c.String(),
                        BanReason = c.String(),
                        ApprovingMembershipUser_Id = c.Guid(),
                        MembershipRole_Id = c.Guid(),
                        MembershipUser_Id = c.Guid(nullable: false),
                        Group_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MembershipUser", t => t.ApprovingMembershipUser_Id)
                .ForeignKey("dbo.MembershipRole", t => t.MembershipRole_Id)
                .ForeignKey("dbo.MembershipUser", t => t.MembershipUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Group", t => t.Group_Id)
                .Index(t => t.ApprovingMembershipUser_Id)
                .Index(t => t.MembershipRole_Id)
                .Index(t => t.MembershipUser_Id)
                .Index(t => t.Group_Id);
            
            AddColumn("dbo.Topic", "Category_Id", c => c.Guid());
            AddColumn("dbo.Group", "PublicGroup", c => c.Boolean(nullable: false));
            AddColumn("dbo.Group", "HiddenGroup", c => c.Boolean(nullable: false));
            AddColumn("dbo.Group", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Group", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Group", "MembershipUser_Id", c => c.Guid());
            CreateIndex("dbo.Topic", "Category_Id");
            CreateIndex("dbo.Group", "MembershipUser_Id");
            AddForeignKey("dbo.Group", "MembershipUser_Id", "dbo.MembershipUser", "Id");
            AddForeignKey("dbo.Topic", "Category_Id", "dbo.Category", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Topic", "Category_Id", "dbo.Category");
            DropForeignKey("dbo.GroupUser", "Group_Id", "dbo.Group");
            DropForeignKey("dbo.GroupUser", "MembershipUser_Id", "dbo.MembershipUser");
            DropForeignKey("dbo.GroupUser", "MembershipRole_Id", "dbo.MembershipRole");
            DropForeignKey("dbo.GroupUser", "ApprovingMembershipUser_Id", "dbo.MembershipUser");
            DropForeignKey("dbo.Group", "MembershipUser_Id", "dbo.MembershipUser");
            DropForeignKey("dbo.Category", "Group_Id", "dbo.Group");
            DropIndex("dbo.GroupUser", new[] { "Group_Id" });
            DropIndex("dbo.GroupUser", new[] { "MembershipUser_Id" });
            DropIndex("dbo.GroupUser", new[] { "MembershipRole_Id" });
            DropIndex("dbo.GroupUser", new[] { "ApprovingMembershipUser_Id" });
            DropIndex("dbo.Group", new[] { "MembershipUser_Id" });
            DropIndex("dbo.Category", new[] { "Group_Id" });
            DropIndex("dbo.Topic", new[] { "Category_Id" });
            DropColumn("dbo.Group", "MembershipUser_Id");
            DropColumn("dbo.Group", "RowVersion");
            DropColumn("dbo.Group", "IsDeleted");
            DropColumn("dbo.Group", "HiddenGroup");
            DropColumn("dbo.Group", "PublicGroup");
            DropColumn("dbo.Topic", "Category_Id");
            DropTable("dbo.GroupUser");
            DropTable("dbo.Category");
        }
    }
}
