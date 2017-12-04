namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InspectionItemCannedResponseRelationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CannedResponses", "InspectionItem_Id", c => c.Guid());
            CreateIndex("dbo.CannedResponses", "InspectionItem_Id");
            AddForeignKey("dbo.CannedResponses", "InspectionItem_Id", "dbo.InspectionItems", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CannedResponses", "InspectionItem_Id", "dbo.InspectionItems");
            DropIndex("dbo.CannedResponses", new[] { "InspectionItem_Id" });
            DropColumn("dbo.CannedResponses", "InspectionItem_Id");
        }
    }
}
