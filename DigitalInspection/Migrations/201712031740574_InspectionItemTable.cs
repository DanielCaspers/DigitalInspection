namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InspectionItemTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InspectionItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.InspectionItems");
        }
    }
}
