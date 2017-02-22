namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Checklists", "Name", c => c.String(nullable: false, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Checklists", "Name", c => c.String(unicode: false));
        }
    }
}
