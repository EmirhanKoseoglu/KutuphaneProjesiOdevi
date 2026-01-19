using Microsoft.AspNetCore.Mvc;
using LibraryMS.Data;
using LibraryMS.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

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
                    .Count(x => x.ReturnDate == null && System.DateTime.Now > x.DueDate)
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
    }
}