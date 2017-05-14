namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixingChecklistModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChecklistItems", "Checklist_Id", "dbo.Checklists");
            DropForeignKey("dbo.ChecklistItems", "Checklist_Id1", "dbo.Checklists");
            DropForeignKey("dbo.Checklists", "ChecklistItem_Id", "dbo.ChecklistItems");
            DropIndex("dbo.ChecklistItems", new[] { "Checklist_Id" });
            DropIndex("dbo.ChecklistItems", new[] { "Checklist_Id1" });
            DropIndex("dbo.Checklists", new[] { "ChecklistItem_Id" });
            CreateTable(
                "dbo.ChecklistChecklistItems",
                c => new
                    {
                        Checklist_Id = c.Guid(nullable: false),
                        ChecklistItem_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Checklist_Id, t.ChecklistItem_Id })
                .ForeignKey("dbo.Checklists", t => t.Checklist_Id, cascadeDelete: true)
                .ForeignKey("dbo.ChecklistItems", t => t.ChecklistItem_Id, cascadeDelete: true)
                .Index(t => t.Checklist_Id)
                .Index(t => t.ChecklistItem_Id);
            
            DropColumn("dbo.ChecklistItems", "Checklist_Id");
            DropColumn("dbo.ChecklistItems", "Checklist_Id1");
            DropColumn("dbo.Checklists", "ChecklistItem_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Checklists", "ChecklistItem_Id", c => c.Guid());
            AddColumn("dbo.ChecklistItems", "Checklist_Id1", c => c.Guid());
            AddColumn("dbo.ChecklistItems", "Checklist_Id", c => c.Guid());
            DropForeignKey("dbo.ChecklistChecklistItems", "ChecklistItem_Id", "dbo.ChecklistItems");
            DropForeignKey("dbo.ChecklistChecklistItems", "Checklist_Id", "dbo.Checklists");
            DropIndex("dbo.ChecklistChecklistItems", new[] { "ChecklistItem_Id" });
            DropIndex("dbo.ChecklistChecklistItems", new[] { "Checklist_Id" });
            DropTable("dbo.ChecklistChecklistItems");
            CreateIndex("dbo.Checklists", "ChecklistItem_Id");
            CreateIndex("dbo.ChecklistItems", "Checklist_Id1");
            CreateIndex("dbo.ChecklistItems", "Checklist_Id");
            AddForeignKey("dbo.Checklists", "ChecklistItem_Id", "dbo.ChecklistItems", "Id");
            AddForeignKey("dbo.ChecklistItems", "Checklist_Id1", "dbo.Checklists", "Id");
            AddForeignKey("dbo.ChecklistItems", "Checklist_Id", "dbo.Checklists", "Id");
        }
    }
}
