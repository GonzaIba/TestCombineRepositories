namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescargasFactura : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DescargasFactura",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CentroComputoIdFK = c.Int(nullable: false),
                        FacturaIdFK = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CentrosDeComputo", t => t.CentroComputoIdFK, cascadeDelete: false)
                .ForeignKey("dbo.Facturas", t => t.FacturaIdFK, cascadeDelete: false)
                .Index(t => t.CentroComputoIdFK)
                .Index(t => t.FacturaIdFK);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DescargasFactura", "FacturaIdFK", "dbo.Facturas");
            DropForeignKey("dbo.DescargasFactura", "CentroComputoIdFK", "dbo.CentrosDeComputo");
            DropIndex("dbo.DescargasFactura", new[] { "FacturaIdFK" });
            DropIndex("dbo.DescargasFactura", new[] { "CentroComputoIdFK" });
            DropTable("dbo.DescargasFactura");
        }
    }
}
