namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init27 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MedKomZayavka", "NaprMedZakazanoDat", c => c.DateTime());
            AddColumn("dbo.MedKomZayavka", "NaprMedPoluchenoDat", c => c.DateTime());
            AddColumn("dbo.Person", "PaspKodPodr", c => c.String());
            AddColumn("dbo.Person", "IsMedKommNeGoden", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "IsMedKommNeGoden");
            DropColumn("dbo.Person", "PaspKodPodr");
            DropColumn("dbo.MedKomZayavka", "NaprMedPoluchenoDat");
            DropColumn("dbo.MedKomZayavka", "NaprMedZakazanoDat");
        }
    }
}
