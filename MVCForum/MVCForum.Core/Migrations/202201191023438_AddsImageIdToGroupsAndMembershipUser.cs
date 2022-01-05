namespace MvcForum.Core.Services.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddsImageIdToGroupsAndMembershipUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Group", "FK_dbo.Group_dbo.Image_HeaderImage");
            AddColumn("dbo.MembershipUser", "ImageId", c => c.Guid());
            RenameColumn("dbo.Group", "HeaderImage", "ImageId");
            AddForeignKey("dbo.Group", "ImageId", "dbo.Image", "Id", false, "FK_dbo.Group_dbo.Image_ImageId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Group", "FK_dbo.Group_dbo.Image_ImageId");
            RenameColumn("dbo.Group", "ImageId", "HeaderImage");
            DropColumn("dbo.MembershipUser", "ImageId");
            AddForeignKey("dbo.Group", "ImageId", "dbo.Image", "HeaderImage", false, "FK_dbo.Group_dbo.Image_HeaderImage");
        }
    }
}
