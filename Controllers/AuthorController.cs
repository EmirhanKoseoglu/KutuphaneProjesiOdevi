using Microsoft.AspNetCore.Mvc;
using LibraryMS.Data;
using LibraryMS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks; 

namespace LibraryMS.Controllers
{
    public class AuthorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostEnvironment;

        public AuthorController(AppDbContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index(string searchString)
        {
            // Arama kutusunda yazılan yazı kaybolmasın diye geri gönderiyoruz
            ViewData["CurrentFilter"] = searchString;

            // Yazarları taslak olarak seçiyoruz
            var authors = from a in _context.Authors
                          select a;

            // Eğer arama kutusu boş değilse filtreliyoruz
            if (!string.IsNullOrEmpty(searchString))
            {
                authors = authors.Where(s => s.Name.Contains(searchString));
            }

            // Sonucu listeye çevirip gönderiyoruz
            return View(authors.ToList());
        }
        // ==========================================

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Author author)
        {
            if (ModelState.IsValid)
            {
                if (author.ImageUpload != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(author.ImageUpload.FileName);
                    string extension = Path.GetExtension(author.ImageUpload.FileName);
                    author.ImageUrl = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/images/", fileName);

                    if (!Directory.Exists(Path.Combine(wwwRootPath, "images")))
                    {
                        Directory.CreateDirectory(Path.Combine(wwwRootPath, "images"));
                    }

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await author.ImageUpload.CopyToAsync(fileStream);
                    }
                }

                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            
            return View(author);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var author = _context.Authors.Find(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var author = _context.Authors.Find(id);
            if (author == null) return NotFound();
            return View(author);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Author author)
        {
            if (ModelState.IsValid)
            {
                if (author.ImageUpload != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(author.ImageUpload.FileName);
                    string extension = Path.GetExtension(author.ImageUpload.FileName);
                    author.ImageUrl = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/images/", fileName);

                    if (!Directory.Exists(Path.Combine(wwwRootPath, "images")))
                    {
                        Directory.CreateDirectory(Path.Combine(wwwRootPath, "images"));
                    }

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await author.ImageUpload.CopyToAsync(fileStream);
                    }
                }

                _context.Authors.Update(author);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(author);
        }
    }
}