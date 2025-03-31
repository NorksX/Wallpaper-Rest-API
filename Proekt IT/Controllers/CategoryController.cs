using Microsoft.AspNet.Identity;
using Proekt_IT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Proekt_IT.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var categories = await db.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(categories);
        }

        public async Task<ActionResult> AddWallpapersToCategory(int categoryId)
        {
            var userId = User.Identity.GetUserId();

            var likedWallpapers = await db.LikedWallpapers
                .Where(lw => lw.UserId == userId)
                .ToListAsync();

            var category = await db.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

            if (category == null)
            {
                return HttpNotFound();
            }

            var model = new AddLikedToCategory
            {
                CategoryId = categoryId,
                CategoryName = category.Name,
                LikedWallpapers = likedWallpapers
            };

            return View(model);
        }
        public async Task<ActionResult> ViewCategory(int categoryId)
        {
            var userId = User.Identity.GetUserId();

            var category = await db.Categories
                .Include(c => c.LikedWallpapers) 
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<ActionResult> AddWallpapersToCategory(int categoryId, List<int> wallpaperIds)
        {
            var userId = User.Identity.GetUserId();

            var category = await db.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

            if (category == null)
            {
                return HttpNotFound();
            }


            var wallpapersToAdd = await db.LikedWallpapers
                .Where(lw => wallpaperIds.Contains(lw.Id) && lw.UserId == userId)
                .ToListAsync();

            foreach (var wallpaper in wallpapersToAdd)
            {
                category.LikedWallpapers.Add(wallpaper);
            }

            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                category.UserId = userId; 

                db.Categories.Add(category);
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(category);
        }


        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var category = await db.Categories.FindAsync(id);
            if (category != null)
            {
                db.Categories.Remove(category);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Category not found." });
        }

        [HttpPost]
        public async Task<ActionResult> RemoveWallpaperFromCategory(int wallpaperId, int categoryId)
        {
            var userId = User.Identity.GetUserId();


            var category = await db.Categories
                .Include(c => c.LikedWallpapers)
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

            if (category == null)
            {
                return HttpNotFound(); 
            }


            var wallpaper = category.LikedWallpapers.FirstOrDefault(w => w.Id == wallpaperId);
            if (wallpaper == null)
            {
                return HttpNotFound(); 
            }


            category.LikedWallpapers.Remove(wallpaper);

            try
            {
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing wallpaper from category: {ex.Message}");
                return Json(new { success = false, message = "Error removing wallpaper." });
            }
        }




    }
}