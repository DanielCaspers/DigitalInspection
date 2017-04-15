namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChecklistItemCannedResponseFK : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CannedResponses",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Response = c.String(nullable: false, unicode: false),
                        Url = c.String(unicode: false),
                        Description = c.String(unicode: false),
                        ChecklistItem_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChecklistItems", t => t.ChecklistItem_Id)
                .Index(t => t.ChecklistItem_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CannedResponses", "ChecklistItem_Id", "dbo.ChecklistItems");
            DropIndex("dbo.CannedResponses", new[] { "ChecklistItem_Id" });
            DropTable("dbo.CannedResponses");
        }
    }
}
