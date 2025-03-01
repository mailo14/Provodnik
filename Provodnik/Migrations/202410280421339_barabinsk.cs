namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class barabinsk : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "IsBarabinsk", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "IsKuibishev", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "IsKuibishev");
            DropColumn("dbo.Person", "IsBarabinsk");
        }
    }
}
