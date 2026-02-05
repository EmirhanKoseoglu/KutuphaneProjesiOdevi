using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryMS.Data;
using LibraryMS.Models;
using System.Security.Claims; 
using Microsoft.AspNetCore.Identity; 

namespace LibraryMS.Controllers
{
    [Authorize] 
    public class BorrowController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BorrowController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public async Task<IActionResult> CreateConfirmed(int bookId, string firstName, string lastName, string phoneNumber, string address)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            // 1. Check if Stock is available
            var book = _context.Books.Find(bookId);
            if (book == null || book.Stock <= 0)
            {
                 TempData["Error"] = "Üzgünüz, bu kitabın stoğu tükenmiştir.";
                 return RedirectToAction("Index", "Book");
            }

            // 1. Check if ANYONE has this book currently borrowed (Stock check)
            // OLD LOGIC: var isBookUnavailable = _context.BorrowRecords.Any(x => x.BookId == bookId && x.ReturnDate == null);
            // NEW LOGIC: We rely on Stock count. But we should still prevent the SAME user from borrowing the SAME book twice if they haven't returned it?
            // Yes, user shouldn't borrow multiple copies of same book unless allowed. Let's keep that check.
            
            var userHasIt = _context.BorrowRecords
                .Any(x => x.BookId == bookId && x.UserId == userId && x.ReturnDate == null);

            if (userHasIt)
            {
                TempData["Error"] = "Bu kitabı zaten ödünç aldınız. Lütfen önce iade ediniz.";
                return RedirectToAction("MyBooks");
            }

            var newLoan = new BorrowRecord
            {
                BookId = bookId,
                UserId = userId,
                LoanDate = DateTime.Now, 
                DueDate = DateTime.Now.AddDays(15),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Address = address
            };

            // Decrement Stock
            book.Stock--;
            _context.Books.Update(book);

            _context.BorrowRecords.Add(newLoan);
            _context.SaveChanges();

            TempData["Success"] = "Kitap ödünç alma işlemi başarıyla gerçekleşti.";
            return RedirectToAction("MyBooks");
        }
        public async Task<IActionResult> MyBooks()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var myBooks = _context.BorrowRecords
                .Include(x => x.Book)
                .ThenInclude(b => b.Author) 
                .Where(x => x.UserId == userId && x.ReturnDate == null)
                .ToList();

            return View(myBooks);
        }

        [HttpPost]
        public IActionResult ReturnBook(int id, string returnUrl = null) 
        {
            var record = _context.BorrowRecords.Find(id);

            if (record != null)
            {
                record.ReturnDate = DateTime.Now; 

                // Increment Stock
                var book = _context.Books.Find(record.BookId);
                if (book != null)
                {
                    book.Stock++;
                    _context.Books.Update(book);
                }

                _context.SaveChanges();
                TempData["Success"] = "Kitap başarıyla iade alındı.";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("MyBooks");
        }
    }
}