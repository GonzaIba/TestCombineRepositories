namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterFacturaQtyDescargaCC : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facturas", "QtyDescargasCC", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Facturas", "QtyDescargasCC");
        }
    }
}
