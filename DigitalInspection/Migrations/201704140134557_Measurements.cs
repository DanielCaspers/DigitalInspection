namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Measurements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Measurements",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Label = c.String(nullable: false, unicode: false),
                        Unit = c.String(nullable: false, unicode: false),
                        ChecklistItem_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChecklistItems", t => t.ChecklistItem_Id)
                .Index(t => t.ChecklistItem_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Measurements", "ChecklistItem_Id", "dbo.ChecklistItems");
            DropIndex("dbo.Measurements", new[] { "ChecklistItem_Id" });
            DropTable("dbo.Measurements");
        }
    }
}
