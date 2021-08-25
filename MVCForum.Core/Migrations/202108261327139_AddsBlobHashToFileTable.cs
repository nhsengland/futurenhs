namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddsBlobHashToFileTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.File", "BlobHash", c => c.Binary(maxLength: 16));
        }
        
        public override void Down()
        {
            DropColumn("dbo.File", "BlobHash");
        }
    }
}
