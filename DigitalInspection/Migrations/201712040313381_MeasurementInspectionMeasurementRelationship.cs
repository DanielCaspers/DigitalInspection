namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MeasurementInspectionMeasurementRelationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspectionMeasurements", "Measurement_Id", c => c.Guid());
            CreateIndex("dbo.InspectionMeasurements", "Measurement_Id");
            AddForeignKey("dbo.InspectionMeasurements", "Measurement_Id", "dbo.Measurements", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InspectionMeasurements", "Measurement_Id", "dbo.Measurements");
            DropIndex("dbo.InspectionMeasurements", new[] { "Measurement_Id" });
            DropColumn("dbo.InspectionMeasurements", "Measurement_Id");
        }
    }
}
