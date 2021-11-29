namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "BadgeRus", c => c.String());
            AddColumn("dbo.Person", "BadgeEng", c => c.String());
            AddColumn("dbo.Person", "Sezon", c => c.DateTime(nullable: false));
            AddColumn("dbo.Person", "NaprMedZakazanoDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsNaprMedZakazano", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "NaprMedPoluchenoDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsNaprMedPolucheno", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "NaprMedVidanoDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsNaprMedVidano", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "VaccineOneDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsVaccineOne", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "VaccineOneOnlyDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsVaccineOneOnly", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "VaccineTwoDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsVaccineTwo", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "RevacDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsExamenFailed", c => c.Boolean(nullable: false));
            AddColumn("dbo.SendGroup", "Name", c => c.String());
            AddColumn("dbo.SendGroup", "PeresadSt", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SendGroup", "PeresadSt");
            DropColumn("dbo.SendGroup", "Name");
            DropColumn("dbo.Person", "IsExamenFailed");
            DropColumn("dbo.Person", "RevacDat");
            DropColumn("dbo.Person", "IsVaccineTwo");
            DropColumn("dbo.Person", "VaccineTwoDat");
            DropColumn("dbo.Person", "IsVaccineOneOnly");
            DropColumn("dbo.Person", "VaccineOneOnlyDat");
            DropColumn("dbo.Person", "IsVaccineOne");
            DropColumn("dbo.Person", "VaccineOneDat");
            DropColumn("dbo.Person", "IsNaprMedVidano");
            DropColumn("dbo.Person", "NaprMedVidanoDat");
            DropColumn("dbo.Person", "IsNaprMedPolucheno");
            DropColumn("dbo.Person", "NaprMedPoluchenoDat");
            DropColumn("dbo.Person", "IsNaprMedZakazano");
            DropColumn("dbo.Person", "NaprMedZakazanoDat");
            DropColumn("dbo.Person", "Sezon");
            DropColumn("dbo.Person", "BadgeEng");
            DropColumn("dbo.Person", "BadgeRus");
        }
    }
}
