namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateFacturas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Facturas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tipo = c.String(maxLength: 50, unicode: false),
                        Numero = c.String(maxLength: 50, unicode: false),
                        Importe = c.String(maxLength: 50, unicode: false),
                        CuitOrigen = c.String(maxLength: 50, unicode: false),
                        CuitDestino = c.String(maxLength: 50, unicode: false),
                        Detalle = c.String(maxLength: 500, unicode: false),
                        Servicio = c.String(maxLength: 50, unicode: false),
                        IvaDiscriminado = c.String(maxLength: 50, unicode: false),
                        Retenciones = c.String(maxLength: 100, unicode: false),
                        Percepciones = c.String(maxLength: 100, unicode: false),
                        ImpuestosNoGravados = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Facturas");
        }
    }
}
