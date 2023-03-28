namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSinArchivoFactura : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facturas", "SinArchivo", c => c.Boolean(nullable: false));
            AddColumn("dbo.Facturas", "Confirmada", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Facturas", "Confirmada");
            DropColumn("dbo.Facturas", "SinArchivo");
        }
    }
}
