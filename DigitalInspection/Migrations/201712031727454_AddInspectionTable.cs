namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInspectionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Inspections",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WorkOrderId = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Inspections");
        }
    }
}
