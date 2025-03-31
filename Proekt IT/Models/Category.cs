using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proekt_IT.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; } 

        public virtual ICollection<LikedWallpaper> LikedWallpapers { get; set; }

    }
}