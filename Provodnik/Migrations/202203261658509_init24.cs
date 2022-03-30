namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init24 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MedKomZayavka", "BolnicaName", c => c.String());
            AddColumn("dbo.MedKomZayavka", "BolnicaAdres", c => c.String());
            AddColumn("dbo.Person", "IsGologram", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "IsGologram");
            DropColumn("dbo.MedKomZayavka", "BolnicaAdres");
            DropColumn("dbo.MedKomZayavka", "BolnicaName");
        }
    }
}
