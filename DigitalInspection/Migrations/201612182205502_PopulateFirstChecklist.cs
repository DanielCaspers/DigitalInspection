namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateFirstChecklist : DbMigration
    {
        public override void Up()
        {
            Sql(
                "INSERT INTO ChecklistItems (Id, Name)" + 
                " VALUES ('be9736e3-4d8c-4a77-a01c-5f99a52c53a2', 'ExhaustLeak')"
            );

            Sql("INSERT INTO Checklists (Id, Name)" + 
                " VALUES ('16612c49-1529-4e28-b6bc-d3fb0ffa3a3a', 'ShitThatBreaksOnDansCar')"
            );

        }

        public override void Down()
        {
        }
    }
}
