namespace Proekt_IT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class categories1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LikedWallpaperCategories",
                c => new
                    {
                        LikedWallpaper_Id = c.Int(nullable: false),
                        Category_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LikedWallpaper_Id, t.Category_Id })
                .ForeignKey("dbo.LikedWallpapers", t => t.LikedWallpaper_Id, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.Category_Id, cascadeDelete: true)
                .Index(t => t.LikedWallpaper_Id)
                .Index(t => t.Category_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LikedWallpaperCategories", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.LikedWallpaperCategories", "LikedWallpaper_Id", "dbo.LikedWallpapers");
            DropIndex("dbo.LikedWallpaperCategories", new[] { "Category_Id" });
            DropIndex("dbo.LikedWallpaperCategories", new[] { "LikedWallpaper_Id" });
            DropTable("dbo.LikedWallpaperCategories");
            DropTable("dbo.Categories");
        }
    }
}
