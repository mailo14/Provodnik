namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init23 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MedKomZayavka", "Depo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MedKomZayavka", "Depo");
        }
    }
}
