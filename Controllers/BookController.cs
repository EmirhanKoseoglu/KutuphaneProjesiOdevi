using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using LibraryMS.Data;
using LibraryMS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace LibraryMS.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }


public IActionResult Index(string searchTerm, int? categoryId)
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
        public IActionResult Create(Book book)
        {
             _context.Books.Add(book);
             _context.SaveChanges();
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
        public IActionResult Edit(Book book)
        {
            _context.Books.Update(book);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}