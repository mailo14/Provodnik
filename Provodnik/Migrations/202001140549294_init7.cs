namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init7 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DocType");
            AlterColumn("dbo.DocType", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.DocType", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DocType");
            AlterColumn("dbo.DocType", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.DocType", "Id");
        }
    }
}
