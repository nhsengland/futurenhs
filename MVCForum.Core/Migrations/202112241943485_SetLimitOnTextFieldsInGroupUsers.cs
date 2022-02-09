namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetLimitOnTextFieldsInGroupUsers : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GroupUser", "RequestToJoinReason", c => c.String(maxLength: 200));
            AlterColumn("dbo.GroupUser", "LockReason", c => c.String(maxLength: 200));
            AlterColumn("dbo.GroupUser", "BanReason", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GroupUser", "BanReason", c => c.String());
            AlterColumn("dbo.GroupUser", "LockReason", c => c.String());
            AlterColumn("dbo.GroupUser", "RequestToJoinReason", c => c.String());
        }
    }
}
