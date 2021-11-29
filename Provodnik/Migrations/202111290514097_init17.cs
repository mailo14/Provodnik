namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SendGroup", "DepoRod", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SendGroup", "DepoRod");
        }
    }
}
