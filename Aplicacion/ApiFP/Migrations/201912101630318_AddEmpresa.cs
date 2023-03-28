namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmpresa : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Empresas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Cuit = c.String(nullable: false, maxLength: 50, unicode: false),
                        Nombre = c.String(maxLength: 200, unicode: false),
                        DomicilioComercial = c.String(maxLength: 500, unicode: false),
                        FechaInsercions = c.DateTime(storeType: "date"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Cuit, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Empresas", new[] { "Cuit" });
            DropTable("dbo.Empresas");
        }
    }
}
