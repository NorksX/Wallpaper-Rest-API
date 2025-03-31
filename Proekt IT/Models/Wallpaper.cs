using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Proekt_IT.Models
{
    public class Wallpaper
    {
        public string Id { get; set; }
        public string Resolution { get; set; }
        public string Creator { get; set; }

        public string ImageUrl { get; set; }

        public string IconUrl { get; set; }

        public string OriginalSource { get; set; }

        [NotMapped] //IsLiked ne e vo databaza
        public bool IsLiked { get; set; }





















        //НЕ СЕ КОРИСТИ

        public virtual ICollection<LikedWallpaper> LikedBy { get; set; }

    }
}