namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMembershipNameFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MembershipUser", "FirstName", c => c.String(nullable: true, maxLength: 255));
            AlterColumn("dbo.MembershipUser", "Surname", c => c.String(nullable: true, maxLength: 255));
            AlterColumn("dbo.MembershipUser", "Initials", c => c.String(nullable: true, maxLength: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MembershipUser", "FirstName", c => c.String());
            AlterColumn("dbo.MembershipUser", "Surname", c => c.String());
            AlterColumn("dbo.MembershipUser", "Initials", c => c.String());
        }
    }
}
