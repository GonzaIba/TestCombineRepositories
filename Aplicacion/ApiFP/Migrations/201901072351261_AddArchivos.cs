namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddArchivos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Archivos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(maxLength: 100, unicode: false),
                        Extension = c.String(maxLength: 10, unicode: false),
                        TipoAlmacenamiento = c.String(maxLength: 50, unicode: false),
                        Volumen = c.String(maxLength: 50, unicode: false),
                        Ruta = c.String(maxLength: 200, unicode: false),
                        FacturaIdFK = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Facturas", t => t.FacturaIdFK, cascadeDelete: true)
                .Index(t => t.FacturaIdFK);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Archivos", "FacturaIdFK", "dbo.Facturas");
            DropIndex("dbo.Archivos", new[] { "FacturaIdFK" });
            DropTable("dbo.Archivos");
        }
    }
}
