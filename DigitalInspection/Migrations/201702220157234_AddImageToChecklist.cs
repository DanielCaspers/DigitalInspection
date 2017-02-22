namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageToChecklist : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Checklists", "Image_Title", c => c.String(unicode: false));
            AddColumn("dbo.Checklists", "Image_Caption", c => c.String(unicode: false));
            AddColumn("dbo.Checklists", "Image_ImageUrl", c => c.String(unicode: false));
            AddColumn("dbo.Checklists", "Image_CreatedDate", c => c.DateTime(nullable: false, precision: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Checklists", "Image_CreatedDate");
            DropColumn("dbo.Checklists", "Image_ImageUrl");
            DropColumn("dbo.Checklists", "Image_Caption");
            DropColumn("dbo.Checklists", "Image_Title");
        }
    }
}
