namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "IsNovichok", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "Dogovor", c => c.String());
            AddColumn("dbo.Person", "DogovorDat", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "DogovorDat");
            DropColumn("dbo.Person", "Dogovor");
            DropColumn("dbo.Person", "IsNovichok");
        }
    }
}
