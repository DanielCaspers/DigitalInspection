namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InspectionMeasurementValueDefaultToNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.InspectionMeasurements", "Value", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.InspectionMeasurements", "Value", c => c.Int(nullable: false));
        }
    }
}
