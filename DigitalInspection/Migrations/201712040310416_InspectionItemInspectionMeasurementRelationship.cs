namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InspectionItemInspectionMeasurementRelationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InspectionMeasurements",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Value = c.Int(nullable: false),
                        InspectionItem_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InspectionItems", t => t.InspectionItem_Id)
                .Index(t => t.InspectionItem_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InspectionMeasurements", "InspectionItem_Id", "dbo.InspectionItems");
            DropIndex("dbo.InspectionMeasurements", new[] { "InspectionItem_Id" });
            DropTable("dbo.InspectionMeasurements");
        }
    }
}
