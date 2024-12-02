using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Data; // Ensure this namespace is included
using PersonalFinanceApp.Models;
using PersonalFinanceApp.Service;
using System.Diagnostics;

namespace PersonalFinanceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;// Inject ApplicationDbContext
        private readonly IUserSessionService _userSessionService;

        // Constructor with Dependency Injection for both ILogger and ApplicationDbContext
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IUserSessionService userSessionService)
        {
            _logger = logger;
            _context = context;
            _userSessionService = userSessionService;
        }
        public IActionResult Index()
        {
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

        public IActionResult Dashboard()
        {
            // Step 1: Get the logged-in user's ID
            int userId = _userSessionService.GetLoggedInUserId();

            if (userId == -1)
            {
                // If no user is logged in, redirect to the login page
                return RedirectToAction("Login", "User");
            }

            // Step 2: Filter the data based on the userId

            // Get the total balance for the logged-in user's accounts
            var totalBalance = _context.Accounts
                .Where(a => a.UserId == userId)  // Filter by userId
                .Sum(a => a.Balance);

            // Get the total transaction count for the logged-in user
            var transactionCount = _context.Transactions
                .Where(t => t.UserId == userId)  // Filter by userId
                .Count();

            // Get the total project count for the logged-in user
            var projectCount = _context.Projects
                .Where(p => p.UserId == userId)  // Filter by userId
                .Count();

            // Get the 5 most recent transactions for the logged-in user
            var recentTransactions = _context.Transactions
                .Where(t => t.UserId == userId)  // Filter by userId
                .OrderByDescending(t => t.Date)
                .Take(5)
                .Include(t => t.Account)
                .Include(t => t.Category)
                .ToList();

            // Step 3: Assign the filtered data to ViewBag
            ViewBag.TotalBalance = totalBalance;
            ViewBag.TransactionCount = transactionCount;
            ViewBag.ProjectCount = projectCount;
            ViewBag.RecentTransactions = recentTransactions;

            // Step 4: Return the view with the filtered data
            return View();
        }


    }
}
