namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SendGroupPerson",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SendGroupId = c.Int(nullable: false),
                        PersonId = c.Int(nullable: false),
                        IsMain = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SendGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        City = c.String(),
                        Depo = c.String(),
                        RegOtdelenie = c.String(),
                        Poezd = c.String(),
                        Vagon = c.String(),
                        OtprDat = c.DateTime(),
                        PribDat = c.DateTime(),
                        PribTime = c.DateTime(),
                        Vstrechat = c.Boolean(nullable: false),
                        Vokzal = c.String(),
                        Marshrut = c.String(),
                        Uvolnenie = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SendGroup");
            DropTable("dbo.SendGroupPerson");
        }
    }
}
