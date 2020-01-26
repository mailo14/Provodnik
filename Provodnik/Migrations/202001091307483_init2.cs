namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "Otryad", c => c.String());
            AddColumn("dbo.Person", "UchZavedenie", c => c.String());
            AddColumn("dbo.Person", "UchForma", c => c.String());
            AddColumn("dbo.Person", "UchFac", c => c.String());
            AddColumn("dbo.Person", "UchGod", c => c.String());
            AddColumn("dbo.Person", "RodFio", c => c.String());
            AddColumn("dbo.Person", "RodPhone", c => c.String());
            AddColumn("dbo.Person", "HasForma", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "RazmerFormi", c => c.String());
            AddColumn("dbo.Person", "BirthDat", c => c.DateTime());
            AddColumn("dbo.Person", "MestoRozd", c => c.String());
            AddColumn("dbo.Person", "PaspSeriya", c => c.String());
            AddColumn("dbo.Person", "PaspNomer", c => c.String());
            AddColumn("dbo.Person", "PaspVidan", c => c.String());
            AddColumn("dbo.Person", "VidanDat", c => c.DateTime());
            AddColumn("dbo.Person", "PaspAdres", c => c.String());
            AddColumn("dbo.Person", "VremRegDat", c => c.DateTime());
            AddColumn("dbo.Person", "FactAdres", c => c.String());
            AddColumn("dbo.Person", "Snils", c => c.String());
            AddColumn("dbo.Person", "Inn", c => c.String());
            AddColumn("dbo.Person", "PsihDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsPsih", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "IsPsihZabral", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "SanKnizkaDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsSanKnizka", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "MedKommDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsMedKomm", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "UchebCentr", c => c.String());
            AddColumn("dbo.Person", "UchebStartDat", c => c.DateTime());
            AddColumn("dbo.Person", "UchebEndDat", c => c.DateTime());
            AddColumn("dbo.Person", "PraktikaDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsPraktika", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "ExamenDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsExamen", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "Srez", c => c.String());
            AddColumn("dbo.Person", "VihodDat", c => c.DateTime());
            AddColumn("dbo.Person", "Gorod", c => c.String());
            AddColumn("dbo.Person", "IsVibil", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "VibilPrichina", c => c.String());
            AddColumn("dbo.Person", "AllPasport", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "AllScans", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "Messages", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "Messages");
            DropColumn("dbo.Person", "AllScans");
            DropColumn("dbo.Person", "AllPasport");
            DropColumn("dbo.Person", "VibilPrichina");
            DropColumn("dbo.Person", "IsVibil");
            DropColumn("dbo.Person", "Gorod");
            DropColumn("dbo.Person", "VihodDat");
            DropColumn("dbo.Person", "Srez");
            DropColumn("dbo.Person", "IsExamen");
            DropColumn("dbo.Person", "ExamenDat");
            DropColumn("dbo.Person", "IsPraktika");
            DropColumn("dbo.Person", "PraktikaDat");
            DropColumn("dbo.Person", "UchebEndDat");
            DropColumn("dbo.Person", "UchebStartDat");
            DropColumn("dbo.Person", "UchebCentr");
            DropColumn("dbo.Person", "IsMedKomm");
            DropColumn("dbo.Person", "MedKommDat");
            DropColumn("dbo.Person", "IsSanKnizka");
            DropColumn("dbo.Person", "SanKnizkaDat");
            DropColumn("dbo.Person", "IsPsihZabral");
            DropColumn("dbo.Person", "IsPsih");
            DropColumn("dbo.Person", "PsihDat");
            DropColumn("dbo.Person", "Inn");
            DropColumn("dbo.Person", "Snils");
            DropColumn("dbo.Person", "FactAdres");
            DropColumn("dbo.Person", "VremRegDat");
            DropColumn("dbo.Person", "PaspAdres");
            DropColumn("dbo.Person", "VidanDat");
            DropColumn("dbo.Person", "PaspVidan");
            DropColumn("dbo.Person", "PaspNomer");
            DropColumn("dbo.Person", "PaspSeriya");
            DropColumn("dbo.Person", "MestoRozd");
            DropColumn("dbo.Person", "BirthDat");
            DropColumn("dbo.Person", "RazmerFormi");
            DropColumn("dbo.Person", "HasForma");
            DropColumn("dbo.Person", "RodPhone");
            DropColumn("dbo.Person", "RodFio");
            DropColumn("dbo.Person", "UchGod");
            DropColumn("dbo.Person", "UchFac");
            DropColumn("dbo.Person", "UchForma");
            DropColumn("dbo.Person", "UchZavedenie");
            DropColumn("dbo.Person", "Otryad");
        }
    }
}
