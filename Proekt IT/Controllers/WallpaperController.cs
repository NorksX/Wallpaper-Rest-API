using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Proekt_IT.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Proekt_IT.Controllers
{
    public class WallpaperController : Controller
    {
        private readonly HttpClient _httpClient;
        private ApplicationDbContext db = new ApplicationDbContext();

       public WallpaperController()
        {
            _httpClient = new HttpClient();
        }
       


       

        [Authorize]
        public async Task<ActionResult> LikedWallpapers()
        {
            var userId = User.Identity.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home");
            }


            var likedWallpapers = await db.LikedWallpapers
                .Where(l => l.UserId == userId)
                .ToListAsync();

            return View("LikedWallpapers", likedWallpapers);
        }




        [HttpPost]
        public async Task<ActionResult> GetJson(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("TestId");
            }

            try
            {
                Wallpaper wallpaper = await GetWallpaperDetailsFromApi(id);
                return View("Details", wallpaper);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("TestId");
            }
        }

         private async Task<Wallpaper> GetWallpaperDetailsFromApi(string wallpaperId)
        {
            if (string.IsNullOrEmpty(wallpaperId))
            {
                throw new ArgumentException("Wallpaper ID cannot be null or empty", nameof(wallpaperId));
            }

            string url = $"https://wallhaven.cc/api/v1/w/{wallpaperId}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                JObject jsonData = JObject.Parse(jsonString);

                return new Wallpaper
                {
                    Id = (string)jsonData["data"]["id"],
                    Resolution = (string)jsonData["data"]["resolution"],
                    Creator = (string)jsonData["data"]["uploader"]["username"],
                    IconUrl = (string)jsonData["data"]["path"],
                    ImageUrl = (string)jsonData["data"]["thumbs"]["original"],
                    OriginalSource = (string)jsonData["data"]["source"]
            };
            }
            catch (HttpRequestException e)
            {
                throw new ApplicationException($"Error retrieving data: {e.Message}", e);
            }
        }
        
        [HttpPost]
        //purity=100&categories=100
        public async Task<ActionResult> WallpaperSearch(string query, int page = 1, string resolution = "1920x1080")
        {
            var userId = User.Identity.GetUserId();
        
            if (string.IsNullOrEmpty(query))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid query parameter");
            }

            string apiUrl = $"https://wallhaven.cc/api/v1/search?q={HttpUtility.UrlEncode(query)}&purity=100&categories=101&resolutions={resolution}&page={page}";
            List<Wallpaper> wallpapers = await FetchWallpapersFromApi(apiUrl);

            var likedWallpapers = await db.LikedWallpapers
                .Where(l => l.UserId == userId)
                .Select(l => l.WallpaperId)
                .ToListAsync();

            foreach (var wallpaper in wallpapers)
            {
                wallpaper.IsLiked = likedWallpapers.Contains(wallpaper.Id);
            }

            ViewBag.Resolution = resolution;
            ViewBag.apiUrl = apiUrl;
            ViewBag.Page = page;
            ViewBag.Query = query;
            ViewBag.LikedWallpapers = likedWallpapers;

            //НАЈБИТНА ЛИНИЈА КОД
            ModelState.Remove("page");

            return View("SearchResults", wallpapers);
        }
        [Authorize]
        [HttpPost]
        public async Task<JsonResult> LikeWallpaper(string wallpaperId)
        {
            var userId = User.Identity.GetUserId();

            if (string.IsNullOrEmpty(wallpaperId))
            {
                return Json(new { success = false, message = "Invalid wallpaper ID" });
            }

            var likedWallpaper = await db.LikedWallpapers
                .FirstOrDefaultAsync(l => l.UserId == userId && l.WallpaperId == wallpaperId);

            if (likedWallpaper == null)
            {
                try
                {
                    Wallpaper wallpaper = await GetWallpaperDetailsFromApi(wallpaperId);

                    likedWallpaper = new LikedWallpaper
                    {
                        UserId = userId,
                        WallpaperId = wallpaperId,
                        Resolution = wallpaper.Resolution,
                        Creator = wallpaper.Creator,
                        IconUrl = wallpaper.IconUrl,
                        ImageUrl = wallpaper.ImageUrl,
                        OriginalSource = wallpaper.OriginalSource,
                    };

                    db.LikedWallpapers.Add(likedWallpaper);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error retrieving wallpaper details: {ex.Message}" });
                }
            }
            else
            {
                db.LikedWallpapers.Remove(likedWallpaper);
            }

            await db.SaveChangesAsync();

            return Json(new { success = true });
        }
       

   

        public ActionResult SearchResults(List<Wallpaper> wallpapers)
        {
            return View(wallpapers);
        }
        public ActionResult Results(Wallpaper wallpaper)
        {
            return View(wallpaper);
        }
    

        // GET: Wallpaper/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        [HttpGet]
        public ActionResult TestID()
        {
            return View();
        }




        //ПОМОШНА НА SEARCH
        private async Task<List<Wallpaper>> FetchWallpapersFromApi(string apiUrl)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch wallpapers. Status code: {response.StatusCode}");
            }

            string jsonString = await response.Content.ReadAsStringAsync();
            JObject jsonData = JObject.Parse(jsonString);
            var wallpapers = new List<Wallpaper>();

            foreach (var item in jsonData["data"])
            {
                var wallpaper = new Wallpaper
                {
                    Id = (string)item["id"],
                    ImageUrl = (string)item["thumbs"]["original"],
                    Resolution = (string)item["resolution"],
                    Creator = (string)item["uploader"]?["username"] ?? "Unknown",
                    OriginalSource = (string)item["source"] ?? "Unknown"
                };

                wallpapers.Add(wallpaper);
            }

            return wallpapers;
        }
        
        //НЕКОРИСТЕНО, ЛОГИКАТ ЗА LIKE И UNLIKE СЕ ВРШИ ВО LikeWallpaper АКЦИЈАТА.
        [Authorize]
        [HttpPost]
        public async Task<JsonResult> UnlikeWallpaper(int id)
        {
            var likedWallpaper = db.LikedWallpapers.FirstOrDefault(w => w.Id == id);

            if (likedWallpaper == null)
            {
                return Json(new { success = false, message = "Wallpaper not found." });
            }

            db.LikedWallpapers.Remove(likedWallpaper);
            await db.SaveChangesAsync();

            return Json(new { success = true, message = "Wallpaper successfully removed from liked." });
        }

    }
}
