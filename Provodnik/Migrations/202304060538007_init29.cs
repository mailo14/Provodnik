namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init29 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "InSpisokSb", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "InSpisokSb");
        }
    }
}
