namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterFacturaDomicilioFiscal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facturas", "DomicilioComercial", c => c.String(maxLength: 250, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Facturas", "DomicilioComercial");
        }
    }
}
