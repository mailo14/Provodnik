namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init25 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SendGroup", "Sp", c => c.String());
            AddColumn("dbo.SendGroup", "Filial", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SendGroup", "Filial");
            DropColumn("dbo.SendGroup", "Sp");
        }
    }
}
