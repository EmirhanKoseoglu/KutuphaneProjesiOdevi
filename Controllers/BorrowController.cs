using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryMS.Data;
using LibraryMS.Models;
using System.Security.Claims; 

namespace LibraryMS.Controllers
{
    [Authorize] 
    public class BorrowController : Controller
    {
        private readonly AppDbContext _context;

        public BorrowController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create(int bookId)
        {
            var book = _context.Books
                .Include(b => b.Author)
                .FirstOrDefault(b => b.Id == bookId);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost]

        public IActionResult CreateConfirmed(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isBorrowed = _context.BorrowRecords
                .Any(x => x.BookId == bookId && x.ReturnDate == null);

            if (isBorrowed)
            {
                return RedirectToAction("MyBooks");
            }

            var newLoan = new BorrowRecord
            {
                BookId = bookId,
                UserId = userId,

                LoanDate = DateTime.Now, 

                DueDate = DateTime.Now.AddDays(15) 
            };

            _context.BorrowRecords.Add(newLoan);
            _context.SaveChanges();

            return RedirectToAction("MyBooks");
        }
        public IActionResult MyBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myBooks = _context.BorrowRecords
                .Include(x => x.Book)
                .ThenInclude(b => b.Author) 
                .Where(x => x.UserId == userId && x.ReturnDate == null)
                .ToList();

            return View(myBooks);
        }

        [HttpPost]
        public IActionResult ReturnBook(int id) 
        {
            var record = _context.BorrowRecords.Find(id);

            if (record != null)
            {
                record.ReturnDate = DateTime.Now; 
                _context.SaveChanges();
            }

            return RedirectToAction("MyBooks");
        }
    }
}