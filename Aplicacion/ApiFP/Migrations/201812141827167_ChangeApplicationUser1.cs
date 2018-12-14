namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeApplicationUser1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Cuit", c => c.Long(nullable: false));
            AddColumn("dbo.AspNetUsers", "BusinessName", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "Profile", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "Category", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Category");
            DropColumn("dbo.AspNetUsers", "Profile");
            DropColumn("dbo.AspNetUsers", "BusinessName");
            DropColumn("dbo.AspNetUsers", "Cuit");
        }
    }
}
