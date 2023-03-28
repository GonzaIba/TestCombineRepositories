namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateFactura : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facturas", "Fecha", c => c.DateTime(storeType: "date"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Facturas", "Fecha");
        }
    }
}
