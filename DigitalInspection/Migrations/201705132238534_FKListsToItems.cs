namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FKListsToItems : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChecklistItems", "Checklist_Id", "dbo.Checklists");
            AddColumn("dbo.ChecklistItems", "Checklist_Id1", c => c.Guid());
            AddColumn("dbo.Checklists", "ChecklistItem_Id", c => c.Guid());
            CreateIndex("dbo.ChecklistItems", "Checklist_Id1");
            CreateIndex("dbo.Checklists", "ChecklistItem_Id");
            AddForeignKey("dbo.ChecklistItems", "Checklist_Id", "dbo.Checklists", "Id");
            AddForeignKey("dbo.Checklists", "ChecklistItem_Id", "dbo.ChecklistItems", "Id");
            AddForeignKey("dbo.ChecklistItems", "Checklist_Id1", "dbo.Checklists", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChecklistItems", "Checklist_Id1", "dbo.Checklists");
            DropForeignKey("dbo.Checklists", "ChecklistItem_Id", "dbo.ChecklistItems");
            DropForeignKey("dbo.ChecklistItems", "Checklist_Id", "dbo.Checklists");
            DropIndex("dbo.Checklists", new[] { "ChecklistItem_Id" });
            DropIndex("dbo.ChecklistItems", new[] { "Checklist_Id1" });
            DropColumn("dbo.Checklists", "ChecklistItem_Id");
            DropColumn("dbo.ChecklistItems", "Checklist_Id1");
            AddForeignKey("dbo.ChecklistItems", "Checklist_Id", "dbo.Checklists", "Id");
        }
    }
}
