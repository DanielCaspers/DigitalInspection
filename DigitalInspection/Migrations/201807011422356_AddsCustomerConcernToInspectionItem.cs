namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddsCustomerConcernToInspectionItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspectionItems", "IsCustomerConcern", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InspectionItems", "IsCustomerConcern");
        }
    }
}
