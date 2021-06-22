namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMemebershipNameFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MembershipUser", "FirstName", c => c.String());
            AddColumn("dbo.MembershipUser", "Surname", c => c.String());
            AddColumn("dbo.MembershipUser", "Initials", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MembershipUser", "Initials");
            DropColumn("dbo.MembershipUser", "Surname");
            DropColumn("dbo.MembershipUser", "FirstName");
        }
    }
}
