namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "Zametki", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "Zametki");
        }
    }
}
