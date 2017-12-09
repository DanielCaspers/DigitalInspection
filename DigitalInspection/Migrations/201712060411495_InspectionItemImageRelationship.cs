namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InspectionItemImageRelationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(unicode: false),
                        ImageUrl = c.String(unicode: false),
                        CreatedDate = c.DateTime(nullable: false, precision: 0),
                        Discriminator = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        InspectionItem_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InspectionItems", t => t.InspectionItem_Id)
                .Index(t => t.InspectionItem_Id);
            
            AddColumn("dbo.Checklists", "Image_Id", c => c.Guid());
            CreateIndex("dbo.Checklists", "Image_Id");
            AddForeignKey("dbo.Checklists", "Image_Id", "dbo.Images", "Id");
            DropColumn("dbo.Checklists", "Image_Title");
            DropColumn("dbo.Checklists", "Image_Caption");
            DropColumn("dbo.Checklists", "Image_ImageUrl");
            DropColumn("dbo.Checklists", "Image_CreatedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Checklists", "Image_CreatedDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.Checklists", "Image_ImageUrl", c => c.String(unicode: false));
            AddColumn("dbo.Checklists", "Image_Caption", c => c.String(unicode: false));
            AddColumn("dbo.Checklists", "Image_Title", c => c.String(unicode: false));
            DropForeignKey("dbo.Checklists", "Image_Id", "dbo.Images");
            DropForeignKey("dbo.Images", "InspectionItem_Id", "dbo.InspectionItems");
            DropIndex("dbo.Images", new[] { "InspectionItem_Id" });
            DropIndex("dbo.Checklists", new[] { "Image_Id" });
            DropColumn("dbo.Checklists", "Image_Id");
            DropTable("dbo.Images");
        }
    }
}
