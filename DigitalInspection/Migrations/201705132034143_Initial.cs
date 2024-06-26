namespace DigitalInspection.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChecklistItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(unicode: false),
                        Checklist_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Checklists", t => t.Checklist_Id)
                .Index(t => t.Checklist_Id);
            
            CreateTable(
                "dbo.CannedResponses",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ChecklistItemId = c.Guid(nullable: false),
                        Response = c.String(nullable: false, unicode: false),
                        Url = c.String(unicode: false),
                        Description = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChecklistItems", t => t.ChecklistItemId, cascadeDelete: true)
                .Index(t => t.ChecklistItemId);
            
            CreateTable(
                "dbo.Measurements",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ChecklistItemId = c.Guid(nullable: false),
                        Label = c.String(nullable: false, unicode: false),
                        MinValue = c.Int(nullable: false),
                        MaxValue = c.Int(nullable: false),
                        StepSize = c.Int(nullable: false),
                        Unit = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChecklistItems", t => t.ChecklistItemId, cascadeDelete: true)
                .Index(t => t.ChecklistItemId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Checklists",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, unicode: false),
                        Image_Title = c.String(unicode: false),
                        Image_Caption = c.String(unicode: false),
                        Image_ImageUrl = c.String(unicode: false),
                        Image_CreatedDate = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Name = c.String(nullable: false, maxLength: 256, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        RoleId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Email = c.String(maxLength: 256, storeType: "nvarchar"),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(unicode: false),
                        SecurityStamp = c.String(unicode: false),
                        PhoneNumber = c.String(unicode: false),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(precision: 0),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ClaimType = c.String(unicode: false),
                        ClaimValue = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ProviderKey = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TagChecklistItems",
                c => new
                    {
                        Tag_Id = c.Guid(nullable: false),
                        ChecklistItem_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.ChecklistItem_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.ChecklistItems", t => t.ChecklistItem_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.ChecklistItem_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ChecklistItems", "Checklist_Id", "dbo.Checklists");
            DropForeignKey("dbo.TagChecklistItems", "ChecklistItem_Id", "dbo.ChecklistItems");
            DropForeignKey("dbo.TagChecklistItems", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.Measurements", "ChecklistItemId", "dbo.ChecklistItems");
            DropForeignKey("dbo.CannedResponses", "ChecklistItemId", "dbo.ChecklistItems");
            DropIndex("dbo.TagChecklistItems", new[] { "ChecklistItem_Id" });
            DropIndex("dbo.TagChecklistItems", new[] { "Tag_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Measurements", new[] { "ChecklistItemId" });
            DropIndex("dbo.CannedResponses", new[] { "ChecklistItemId" });
            DropIndex("dbo.ChecklistItems", new[] { "Checklist_Id" });
            DropTable("dbo.TagChecklistItems");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Checklists");
            DropTable("dbo.Tags");
            DropTable("dbo.Measurements");
            DropTable("dbo.CannedResponses");
            DropTable("dbo.ChecklistItems");
        }
    }
}
