namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterCentroComputo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CentrosDeComputo", "Contacto", c => c.String(maxLength: 150, unicode: false));
            AddColumn("dbo.CentrosDeComputo", "Email", c => c.String(maxLength: 150, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CentrosDeComputo", "Email");
            DropColumn("dbo.CentrosDeComputo", "Contacto");
        }
    }
}
