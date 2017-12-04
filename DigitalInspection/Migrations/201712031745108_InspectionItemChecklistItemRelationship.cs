namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InspectionItemChecklistItemRelationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspectionItems", "ChecklistItem_Id", c => c.Guid());
            CreateIndex("dbo.InspectionItems", "ChecklistItem_Id");
            AddForeignKey("dbo.InspectionItems", "ChecklistItem_Id", "dbo.ChecklistItems", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InspectionItems", "ChecklistItem_Id", "dbo.ChecklistItems");
            DropIndex("dbo.InspectionItems", new[] { "ChecklistItem_Id" });
            DropColumn("dbo.InspectionItems", "ChecklistItem_Id");
        }
    }
}
