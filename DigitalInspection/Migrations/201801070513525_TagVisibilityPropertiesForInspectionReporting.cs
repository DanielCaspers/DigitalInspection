namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TagVisibilityPropertiesForInspectionReporting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tags", "IsVisibleToCustomer", c => c.Boolean(nullable: false));
            AddColumn("dbo.Tags", "IsVisibleToEmployee", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tags", "IsVisibleToEmployee");
            DropColumn("dbo.Tags", "IsVisibleToCustomer");
        }
    }
}
