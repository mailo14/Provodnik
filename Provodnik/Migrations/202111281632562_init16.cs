namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init16 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Person", "Sezon", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Person", "Sezon", c => c.DateTime(nullable: false));
        }
    }
}
