using Microsoft.AspNetCore.Mvc;
using LibraryMS.Data;
using LibraryMS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace LibraryMS.Controllers
{
    public class AuthorController : Controller
    {
        private readonly AppDbContext _context;

        public AuthorController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var authors = _context.Authors.ToList();
            return View(authors);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Authors.Add(author);
                _context.SaveChanges(); // Hata burada çıkıyordu
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
        public IActionResult Edit(Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Authors.Update(author);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(author);
        }
    }
}