namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TagsInChecklistItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tags", "ChecklistItem_Id", c => c.Guid());
            CreateIndex("dbo.Tags", "ChecklistItem_Id");
            AddForeignKey("dbo.Tags", "ChecklistItem_Id", "dbo.ChecklistItems", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tags", "ChecklistItem_Id", "dbo.ChecklistItems");
            DropIndex("dbo.Tags", new[] { "ChecklistItem_Id" });
            DropColumn("dbo.Tags", "ChecklistItem_Id");
        }
    }
}
