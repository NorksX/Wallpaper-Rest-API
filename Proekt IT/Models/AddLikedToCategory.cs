using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proekt_IT.Models
{
    public class AddLikedToCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<LikedWallpaper> LikedWallpapers { get; set; }
    }
}