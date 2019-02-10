namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EstadoFactura : DbMigration
    {
        public string Nombre { get; internal set; }

        public override void Up()
        {
            CreateTable(
                "dbo.EstadoFacturas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Facturas", "FechaCreacion", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("dbo.Facturas", "EstadoFacturaFK", c => c.Int(nullable: true, defaultValue: 1));
            CreateIndex("dbo.Facturas", "EstadoFacturaFK");
            AddForeignKey("dbo.Facturas", "EstadoFacturaFK", "dbo.EstadoFacturas", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Facturas", "EstadoFacturaFK", "dbo.EstadoFacturas");
            DropIndex("dbo.Facturas", new[] { "EstadoFacturaFK" });
            DropColumn("dbo.Facturas", "EstadoFacturaFK");
            DropColumn("dbo.Facturas", "FechaCreacion");
            DropTable("dbo.EstadoFacturas");
        }
    }
}
