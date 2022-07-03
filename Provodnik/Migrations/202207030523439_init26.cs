namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init26 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "IsTrudoustroen", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "IsTrudoustroen");
        }
    }
}
