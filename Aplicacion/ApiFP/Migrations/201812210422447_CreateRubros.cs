namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateRubros : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rubros",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tipo = c.String(maxLength: 50, unicode: false),
                        Nombre = c.String(maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.Tipo, t.Nombre }, unique: true, name: "IX_Rubro");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Rubros", "IX_Rubro");
            DropTable("dbo.Rubros");
        }
    }
}
