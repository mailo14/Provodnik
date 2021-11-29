namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "SertificatDat", c => c.DateTime());
            AddColumn("dbo.Person", "IsSertificatError", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "SertificatError", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "SertificatError");
            DropColumn("dbo.Person", "IsSertificatError");
            DropColumn("dbo.Person", "SertificatDat");
        }
    }
}
