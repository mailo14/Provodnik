namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init22 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "FormaPoluchena", c => c.Boolean(nullable: false));
            DropColumn("dbo.Person", "IsUchFinish");
            DropColumn("dbo.Person", "HasLgota");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Person", "HasLgota", c => c.Boolean(nullable: false));
            AddColumn("dbo.Person", "IsUchFinish", c => c.Boolean(nullable: false));
            DropColumn("dbo.Person", "FormaPoluchena");
        }
    }
}
