namespace DigitalInspection.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddsCannedResponseRecommendedServiceDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CannedResponses", "RecommendedServiceDescription", c => c.String(nullable: false, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CannedResponses", "RecommendedServiceDescription");
        }
    }
}
