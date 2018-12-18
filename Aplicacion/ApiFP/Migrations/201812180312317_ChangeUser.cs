namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String(nullable: false, maxLength: 100, unicode: false));
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String(nullable: false, maxLength: 100, unicode: false));
            AddColumn("dbo.AspNetUsers", "Cuit", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AddColumn("dbo.AspNetUsers", "BusinessName", c => c.String(nullable: false, maxLength: 100, unicode: false));
            AddColumn("dbo.AspNetUsers", "Profile", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AddColumn("dbo.AspNetUsers", "Category", c => c.String(nullable: false, maxLength: 50, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Category");
            DropColumn("dbo.AspNetUsers", "Profile");
            DropColumn("dbo.AspNetUsers", "BusinessName");
            DropColumn("dbo.AspNetUsers", "Cuit");
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "FirstName");
        }
    }
}
