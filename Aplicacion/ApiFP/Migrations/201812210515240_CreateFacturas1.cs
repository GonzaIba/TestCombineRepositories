namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateFacturas1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facturas", "UserIdFK", c => c.String(maxLength: 128));
            CreateIndex("dbo.Facturas", "UserIdFK");
            AddForeignKey("dbo.Facturas", "UserIdFK", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Facturas", "UserIdFK", "dbo.AspNetUsers");
            DropIndex("dbo.Facturas", new[] { "UserIdFK" });
            DropColumn("dbo.Facturas", "UserIdFK");
        }
    }
}
