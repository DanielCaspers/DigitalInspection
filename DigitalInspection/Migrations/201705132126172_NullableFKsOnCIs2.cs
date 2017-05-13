namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableFKsOnCIs2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CannedResponses", "ChecklistItemId", "dbo.ChecklistItems");
            DropForeignKey("dbo.Measurements", "ChecklistItemId", "dbo.ChecklistItems");
            DropIndex("dbo.CannedResponses", new[] { "ChecklistItemId" });
            DropIndex("dbo.Measurements", new[] { "ChecklistItemId" });
            AlterColumn("dbo.CannedResponses", "ChecklistItemId", c => c.Guid());
            AlterColumn("dbo.Measurements", "ChecklistItemId", c => c.Guid());
            CreateIndex("dbo.CannedResponses", "ChecklistItemId");
            CreateIndex("dbo.Measurements", "ChecklistItemId");
            AddForeignKey("dbo.CannedResponses", "ChecklistItemId", "dbo.ChecklistItems", "Id");
            AddForeignKey("dbo.Measurements", "ChecklistItemId", "dbo.ChecklistItems", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Measurements", "ChecklistItemId", "dbo.ChecklistItems");
            DropForeignKey("dbo.CannedResponses", "ChecklistItemId", "dbo.ChecklistItems");
            DropIndex("dbo.Measurements", new[] { "ChecklistItemId" });
            DropIndex("dbo.CannedResponses", new[] { "ChecklistItemId" });
            AlterColumn("dbo.Measurements", "ChecklistItemId", c => c.Guid(nullable: false));
            AlterColumn("dbo.CannedResponses", "ChecklistItemId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Measurements", "ChecklistItemId");
            CreateIndex("dbo.CannedResponses", "ChecklistItemId");
            AddForeignKey("dbo.Measurements", "ChecklistItemId", "dbo.ChecklistItems", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CannedResponses", "ChecklistItemId", "dbo.ChecklistItems", "Id", cascadeDelete: true);
        }
    }
}
