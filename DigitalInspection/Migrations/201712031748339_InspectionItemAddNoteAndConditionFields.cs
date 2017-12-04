namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InspectionItemAddNoteAndConditionFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InspectionItems", "Note", c => c.String(unicode: false));
            AddColumn("dbo.InspectionItems", "Condition", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InspectionItems", "Condition");
            DropColumn("dbo.InspectionItems", "Note");
        }
    }
}
