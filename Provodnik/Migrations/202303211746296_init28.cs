namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init28 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "SanGigObuchenieDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsSanGigObuchenie", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "TrudoustroenDepo", c => c.String());
            AddColumn("dbo.Person", "NaprMedDepo", c => c.String());
            AddColumn("dbo.Person", "NaprMedBolnicaName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "NaprMedBolnicaName");
            DropColumn("dbo.Person", "NaprMedDepo");
            DropColumn("dbo.Person", "TrudoustroenDepo");
            DropColumn("dbo.Person", "IsSanGigObuchenie");
            DropColumn("dbo.Person", "SanGigObuchenieDat");
        }
    }
}
