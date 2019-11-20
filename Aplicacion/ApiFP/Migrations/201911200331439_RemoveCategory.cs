namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCategory : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Category");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Category", c => c.String(nullable: false, maxLength: 50, unicode: false));
        }
    }
}
