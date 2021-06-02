namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatesTopicsPerPageValueOnSettings : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE Settings SET TopicsPerPage = 10 where TopicsPerPage = 20");
        }
        
        public override void Down()
        {
        }
    }
}
