using LibraryMS.Data;
using LibraryMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LibraryMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context; 

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.BookCount = _context.Books.Count();
            ViewBag.AuthorCount = _context.Authors.Count();
            ViewBag.MemberCount = _context.Users.Count(); 
            
            if (User.Identity.IsAuthenticated)
            {
                var userEmail = User.Identity.Name;
                var userId = _context.Users.FirstOrDefault(u => u.UserName == userEmail)?.Id;
                













                
                ViewBag.MyBooksCount = _context.BorrowRecords
                    .Count(x => x.UserId == userId && x.ReturnDate == null);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}