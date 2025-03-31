    using System;
    using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
    using System.Web;

    namespace Proekt_IT.Models
    {
    public class LikedWallpaper
    {
        [Key]
        public int Id { get; set; } // Internal ID
        public string WallpaperId { get; set; } // ID spored wallhaven
        public string UserId { get; set; } //User koj ima lajknato
        public string Resolution { get; set; }
        public string Creator { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public string OriginalSource { get; set; }

        public int CategoryId { get; set; } //Vo koja kategorija pripagja
        public virtual ICollection<Category> Categories { get; set; }

    }

}