namespace Proekt_IT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class categories2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LikedWallpapers", "CategoryId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LikedWallpapers", "CategoryId");
        }
    }
}
