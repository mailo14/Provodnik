namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonDoc", "PrinesetK", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonDoc", "PrinesetK");
        }
    }
}
