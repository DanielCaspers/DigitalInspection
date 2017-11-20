namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CannedResponsesHaveLevelsOfConcern_AsString : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CannedResponses", "LevelsOfConcernInDb", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CannedResponses", "LevelsOfConcernInDb");
        }
    }
}
