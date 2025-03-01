namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init31 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trudoustroistvo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Depo = c.String(),
                        Kolvo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrudoustroistvoPerson",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrudoustroistvoId = c.Int(nullable: false),
                        PersonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TrudoustroistvoPerson");
            DropTable("dbo.Trudoustroistvo");
        }
    }
}
