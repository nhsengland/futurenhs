namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiesBlobHashTypeToBinary : DbMigration
    {
        public override void Up()
        {            
            AlterColumn("dbo.File", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.File", "UploadStatus", c => c.Int());
            AlterColumn("dbo.File", "BlobHash", c => c.Binary(nullable: true, maxLength: 16, storeType: "binary"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.File", "UploadStatus", c => c.Int(nullable: false));
            AlterColumn("dbo.File", "Title", c => c.String());
            AlterColumn("dbo.File", "BlobHash", c => c.Binary(maxLength: 16));
        }
    }
}
