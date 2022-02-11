namespace MvcForum.Core.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatesDiscussionsPerPageValueOnSettings : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE Settings SET DiscussionsPerPage = 10 where DiscussionsPerPage = 20");
        }
        
        public override void Down()
        {
        }
    }
}
