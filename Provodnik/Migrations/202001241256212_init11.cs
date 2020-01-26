namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init11 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.PersonDoc", "PersonId");
            AddForeignKey("dbo.PersonDoc", "PersonId", "dbo.Person", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonDoc", "PersonId", "dbo.Person");
            DropIndex("dbo.PersonDoc", new[] { "PersonId" });
        }
    }
}
