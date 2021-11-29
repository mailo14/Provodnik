namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init15 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "IsNaprMedPoluchenoNePoln", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "IsNaprMedPoluchenoSOshibkoi", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "IsNaprMedPoluchenoSOshibkoi");
            DropColumn("dbo.Person", "IsNaprMedPoluchenoNePoln");
        }
    }
}
