namespace ApiFP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserRubros : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "RubroOperativoFK", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "RubroOperativoDescripcion", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.AspNetUsers", "RubroExpensasFK", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "RubroExpensasDescripcion", c => c.String(maxLength: 100, unicode: false));
            CreateIndex("dbo.AspNetUsers", "RubroOperativoFK");
            CreateIndex("dbo.AspNetUsers", "RubroExpensasFK");
            AddForeignKey("dbo.AspNetUsers", "RubroExpensasFK", "dbo.Rubros", "Id");
            AddForeignKey("dbo.AspNetUsers", "RubroOperativoFK", "dbo.Rubros", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "RubroOperativoFK", "dbo.Rubros");
            DropForeignKey("dbo.AspNetUsers", "RubroExpensasFK", "dbo.Rubros");
            DropIndex("dbo.AspNetUsers", new[] { "RubroExpensasFK" });
            DropIndex("dbo.AspNetUsers", new[] { "RubroOperativoFK" });
            DropColumn("dbo.AspNetUsers", "RubroExpensasDescripcion");
            DropColumn("dbo.AspNetUsers", "RubroExpensasFK");
            DropColumn("dbo.AspNetUsers", "RubroOperativoDescripcion");
            DropColumn("dbo.AspNetUsers", "RubroOperativoFK");
        }
    }
}
