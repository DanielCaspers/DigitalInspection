namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InspectionInspectionItemsRelationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspectionItems", "Inspection_Id", c => c.Guid());
            CreateIndex("dbo.InspectionItems", "Inspection_Id");
            AddForeignKey("dbo.InspectionItems", "Inspection_Id", "dbo.Inspections", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InspectionItems", "Inspection_Id", "dbo.Inspections");
            DropIndex("dbo.InspectionItems", new[] { "Inspection_Id" });
            DropColumn("dbo.InspectionItems", "Inspection_Id");
        }
    }
}
