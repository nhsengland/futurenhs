namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLatLonFromMembershipUser : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MembershipUser", "Latitude");
            DropColumn("dbo.MembershipUser", "Longitude");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MembershipUser", "Longitude", c => c.String(maxLength: 40));
            AddColumn("dbo.MembershipUser", "Latitude", c => c.String(maxLength: 40));
        }
    }
}
