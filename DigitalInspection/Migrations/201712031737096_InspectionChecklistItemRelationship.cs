namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InspectionChecklistItemRelationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InspectionChecklistItems",
                c => new
                    {
                        Inspection_Id = c.Guid(nullable: false),
                        ChecklistItem_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Inspection_Id, t.ChecklistItem_Id })
                .ForeignKey("dbo.Inspections", t => t.Inspection_Id, cascadeDelete: true)
                .ForeignKey("dbo.ChecklistItems", t => t.ChecklistItem_Id, cascadeDelete: true)
                .Index(t => t.Inspection_Id)
                .Index(t => t.ChecklistItem_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InspectionChecklistItems", "ChecklistItem_Id", "dbo.ChecklistItems");
            DropForeignKey("dbo.InspectionChecklistItems", "Inspection_Id", "dbo.Inspections");
            DropIndex("dbo.InspectionChecklistItems", new[] { "ChecklistItem_Id" });
            DropIndex("dbo.InspectionChecklistItems", new[] { "Inspection_Id" });
            DropTable("dbo.InspectionChecklistItems");
        }
    }
}
