namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NegativeMeasurements : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Measurements", "MinValue", c => c.Int(nullable: false));
            AddColumn("dbo.Measurements", "MaxValue", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Measurements", "MaxValue");
            DropColumn("dbo.Measurements", "MinValue");
        }
    }
}
