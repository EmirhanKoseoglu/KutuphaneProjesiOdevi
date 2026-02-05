using Microsoft.AspNetCore.Mvc;
using LibraryMS.Data;
using LibraryMS.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalBooks = _context.Books.Count(),
                TotalAuthors = _context.Authors.Count(),
                TotalCategories = _context.Categories.Count(),
                
                ActiveLoans = _context.BorrowRecords.Count(x => x.ReturnDate == null),

                OverdueBooks = _context.BorrowRecords
                    .Count(x => x.ReturnDate == null && System.DateTime.Now > x.DueDate),

                RecentTransactions = (from record in _context.BorrowRecords
                                      join user in _context.Users on record.UserId equals user.Id
                                      join book in _context.Books on record.BookId equals book.Id
                                      orderby record.LoanDate descending
                                      select new TransactionInfo
                                      {
                                          Id = record.Id,
                                          MemberName = user.UserName, // Using UserName or Email
                                          BookTitle = book.Title,
                                          LoanDate = record.LoanDate,
                                          DueDate = record.DueDate,
                                          Status = record.ReturnDate == null ? (System.DateTime.Now > record.DueDate ? "Gecikmiş" : "Okunuyor") : "İade Edildi"
                                      }).Take(10).ToList()
            };

            return View(model);
        }

public IActionResult OverdueBooks()
{
    
    var overdueList = (from record in _context.BorrowRecords
                       join user in _context.Users on record.UserId equals user.Id 
                       join book in _context.Books on record.BookId equals book.Id 
                       where record.ReturnDate == null && record.DueDate < DateTime.Now
                       select new OverdueInfo
                       {
                           RecordId = record.Id,
                           BookTitle = book.Title, 
                           MemberEmail = user.Email,
                           DueDate = record.DueDate,
                           OverdueDays = (DateTime.Now - record.DueDate).Days 
                       }).ToList();

    return View(overdueList);
}
        public IActionResult BorrowedBooks()
        {
            var borrowedList = (from record in _context.BorrowRecords
                                join user in _context.Users on record.UserId equals user.Id
                                join book in _context.Books on record.BookId equals book.Id
                                where record.ReturnDate == null
                                orderby record.LoanDate descending
                                select new TransactionInfo
                                {
                                    Id = record.Id,
                                    MemberName = user.UserName,
                                    BookTitle = book.Title,
                                    LoanDate = record.LoanDate,
                                    DueDate = record.DueDate,
                                    Status = System.DateTime.Now > record.DueDate ? "Gecikmiş" : "Okunuyor"
                                }).ToList();

            return View(borrowedList);
        }

        public IActionResult Orders(string searchTerm)
        {
            var ordersQuery = _context.BorrowRecords
                .Include(b => b.Book)
                .Include(u => u.User)
                .OrderByDescending(r => r.LoanDate)
                .Where(r => r.Address != null) // Only show records with delivery info (Orders)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                ordersQuery = ordersQuery.Where(r => 
                    r.FirstName.Contains(searchTerm) || 
                    r.LastName.Contains(searchTerm) ||
                    r.User.UserName.Contains(searchTerm) ||
                    (r.FirstName + " " + r.LastName).Contains(searchTerm));
                
                ViewData["CurrentSearch"] = searchTerm;
            }

            var orders = ordersQuery.ToList();

            return View(orders);
        }
    }
}