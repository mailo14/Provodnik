namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init19 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "VaccineSert", c => c.String());
            AddColumn("dbo.Person", "VaccineSertDat", c => c.DateTime());
            AddColumn("dbo.Person", "VaccineSertDatTo", c => c.DateTime());
            DropColumn("dbo.Person", "VaccineOneDat");
            DropColumn("dbo.Person", "IsVaccineOne");
            DropColumn("dbo.Person", "VaccineOneOnlyDat");
            DropColumn("dbo.Person", "IsVaccineOneOnly");
            DropColumn("dbo.Person", "VaccineTwoDat");
            DropColumn("dbo.Person", "IsVaccineTwo");
            DropColumn("dbo.Person", "RevacDat");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Person", "RevacDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsVaccineTwo", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "VaccineTwoDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsVaccineOneOnly", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "VaccineOneOnlyDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsVaccineOne", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "VaccineOneDat", c => c.DateTime());
            DropColumn("dbo.Person", "VaccineSertDatTo");
            DropColumn("dbo.Person", "VaccineSertDat");
            DropColumn("dbo.Person", "VaccineSert");
        }
    }
}
