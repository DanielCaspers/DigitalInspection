namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedInspectionItemCannedResponseRelationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CannedResponses", "InspectionItem_Id", "dbo.InspectionItems");
            DropIndex("dbo.CannedResponses", new[] { "InspectionItem_Id" });
            CreateTable(
                "dbo.InspectionItemCannedResponses",
                c => new
                    {
                        InspectionItem_Id = c.Guid(nullable: false),
                        CannedResponse_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.InspectionItem_Id, t.CannedResponse_Id })
                .ForeignKey("dbo.InspectionItems", t => t.InspectionItem_Id, cascadeDelete: true)
                .ForeignKey("dbo.CannedResponses", t => t.CannedResponse_Id, cascadeDelete: true)
                .Index(t => t.InspectionItem_Id)
                .Index(t => t.CannedResponse_Id);
            
            DropColumn("dbo.CannedResponses", "InspectionItem_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CannedResponses", "InspectionItem_Id", c => c.Guid());
            DropForeignKey("dbo.InspectionItemCannedResponses", "CannedResponse_Id", "dbo.CannedResponses");
            DropForeignKey("dbo.InspectionItemCannedResponses", "InspectionItem_Id", "dbo.InspectionItems");
            DropIndex("dbo.InspectionItemCannedResponses", new[] { "CannedResponse_Id" });
            DropIndex("dbo.InspectionItemCannedResponses", new[] { "InspectionItem_Id" });
            DropTable("dbo.InspectionItemCannedResponses");
            CreateIndex("dbo.CannedResponses", "InspectionItem_Id");
            AddForeignKey("dbo.CannedResponses", "InspectionItem_Id", "dbo.InspectionItems", "Id");
        }
    }
}
