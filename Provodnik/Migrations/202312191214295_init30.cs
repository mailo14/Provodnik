namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init30 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "IsSvoyaSanKnizka", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "IsSvoyaSanKnizka");
        }
    }
}
