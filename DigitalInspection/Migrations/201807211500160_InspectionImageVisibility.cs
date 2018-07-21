namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InspectionImageVisibility : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Images", "IsVisibleToCustomer", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Images", "IsVisibleToCustomer");
        }
    }
}
