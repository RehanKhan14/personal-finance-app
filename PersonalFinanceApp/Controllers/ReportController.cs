using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using PersonalFinanceApp.Service;

namespace PersonalFinanceApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserSessionService _userSessionService;

        // Constructor injection of the context and session service
        public ReportController(ApplicationDbContext context, IUserSessionService userSessionService)
        {
            _context = context;
            _userSessionService = userSessionService;
        }

        public IActionResult Index(string account, string category, DateTime? startDate, DateTime? endDate)
        {
            // Step 1: Get the logged-in user's ID
            int userId = _userSessionService.GetLoggedInUserId();

            if (userId == -1)
            {
                // If the user is not logged in, redirect to the login page
                return RedirectToAction("Login", "User");
            }

            // Step 2: Start building the query for transactions
            var transactions = _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)  // Filter by userId
                .AsQueryable();

            // Step 3: Apply additional filters based on provided parameters
            if (!string.IsNullOrEmpty(account))
                transactions = transactions.Where(t => t.Account.Name == account);

            if (!string.IsNullOrEmpty(category))
                transactions = transactions.Where(t => t.Category.Name == category);

            if (startDate.HasValue)
                transactions = transactions.Where(t => t.Date >= startDate);

            if (endDate.HasValue)
                transactions = transactions.Where(t => t.Date <= endDate);

            // Step 4: Return the filtered transactions to the view
            return View(transactions.ToList());
        }
    }
}
