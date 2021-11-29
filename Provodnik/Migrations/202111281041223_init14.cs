namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init14 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MedKomZayavkaPerson",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MedKomZayavkaId = c.Int(nullable: false),
                        PersonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MedKomZayavka",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Dat = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MedKomZayavka");
            DropTable("dbo.MedKomZayavkaPerson");
        }
    }
}
