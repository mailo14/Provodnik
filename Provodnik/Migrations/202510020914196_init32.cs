namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init32 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "Tg", c => c.String());
            AddColumn("dbo.Person", "IsKruglogodOtryad", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "IsKruglogodOtryad");
            DropColumn("dbo.Person", "Tg");
        }
    }
}
