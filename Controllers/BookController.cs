using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using LibraryMS.Data;
using LibraryMS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace LibraryMS.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostEnvironment;

        public BookController(AppDbContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }


public IActionResult Index(string searchTerm, int? categoryId, int? authorId)
{
    var booksQuery = _context.Books
        .Include(b => b.Category)
        .Include(b => b.Author)
        .AsQueryable();

    if (!string.IsNullOrEmpty(searchTerm))
    {
        booksQuery = booksQuery.Where(b => 
            b.Title.Contains(searchTerm) || 
            b.Author.Name.Contains(searchTerm));
        
        ViewData["CurrentSearch"] = searchTerm;
    }

    if (categoryId.HasValue)
    {
        booksQuery = booksQuery.Where(b => b.CategoryId == categoryId);
    }

    if (authorId.HasValue)
    {
        booksQuery = booksQuery.Where(b => b.AuthorId == authorId);
    }

    var books = booksQuery.ToList();

    var borrowedBookIds = _context.BorrowRecords
        .Where(x => x.ReturnDate == null)
        .Select(x => x.BookId)
        .ToList();

    ViewBag.BorrowedBookIds = borrowedBookIds;

    return View(books);
}
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Authors = new SelectList(_context.Authors.ToList(), "Id", "Name");
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Book book)
        {
             if (book.ImageUpload != null)
             {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(book.ImageUpload.FileName);
                string extension = Path.GetExtension(book.ImageUpload.FileName);
                book.ImageUrl = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/images/", fileName);
                
                if (!Directory.Exists(Path.Combine(wwwRootPath, "images")))
                {
                    Directory.CreateDirectory(Path.Combine(wwwRootPath, "images"));
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await book.ImageUpload.CopyToAsync(fileStream);
                }
             }

             _context.Books.Add(book);
             await _context.SaveChangesAsync();
             return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            var authorList = _context.Authors.ToList();
            var categoryList = _context.Categories.ToList();

            ViewBag.Authors = new SelectList(authorList, "Id", "Name", book.AuthorId);
            ViewBag.Categories = new SelectList(categoryList, "Id", "Name", book.CategoryId);

            return View(book);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Book book)
        {
             if (book.ImageUpload != null)
             {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(book.ImageUpload.FileName);
                string extension = Path.GetExtension(book.ImageUpload.FileName);
                book.ImageUrl = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/images/", fileName);
                
                if (!Directory.Exists(Path.Combine(wwwRootPath, "images")))
                {
                    Directory.CreateDirectory(Path.Combine(wwwRootPath, "images"));
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await book.ImageUpload.CopyToAsync(fileStream);
                }
             }

             _context.Books.Update(book);
             await _context.SaveChangesAsync();
             return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var book = _context.Books.Find(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}