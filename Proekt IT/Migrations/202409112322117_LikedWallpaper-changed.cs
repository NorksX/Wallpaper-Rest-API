namespace Proekt_IT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LikedWallpaperchanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LikedWallpapers", "Resolution", c => c.String());
            AddColumn("dbo.LikedWallpapers", "Creator", c => c.String());
            AddColumn("dbo.LikedWallpapers", "IconUrl", c => c.String());
            AddColumn("dbo.LikedWallpapers", "ImageUrl", c => c.String());
            AddColumn("dbo.LikedWallpapers", "OriginalSource", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LikedWallpapers", "OriginalSource");
            DropColumn("dbo.LikedWallpapers", "ImageUrl");
            DropColumn("dbo.LikedWallpapers", "IconUrl");
            DropColumn("dbo.LikedWallpapers", "Creator");
            DropColumn("dbo.LikedWallpapers", "Resolution");
        }
    }
}
