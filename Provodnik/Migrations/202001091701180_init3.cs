﻿namespace Provodnik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SendGroup", "PribTime", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SendGroup", "PribTime", c => c.DateTime());
        }
    }
}
