namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPronounsToMembershipTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MembershipUser", "Pronouns", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MembershipUser", "Pronouns");
        }
    }
}
