namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCentrosDeComputo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CentrosDeComputo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(maxLength: 50, unicode: false),
                        ApiKey = c.String(maxLength: 32, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CentrosDeComputo");
        }
    }
}
