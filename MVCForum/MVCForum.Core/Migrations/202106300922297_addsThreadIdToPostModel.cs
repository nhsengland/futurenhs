namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsThreadIdToPostModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Post", "ThreadId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Post", "ThreadId");
        }
    }
}
