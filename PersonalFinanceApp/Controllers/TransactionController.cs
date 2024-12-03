using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Data;
using PersonalFinanceApp.Models;
using PersonalFinanceApp.Service;

namespace PersonalFinanceApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserSessionService _userSessionService;

        public TransactionController(ApplicationDbContext context, IUserSessionService userSessionService)
        {
            _context = context;
            _userSessionService = userSessionService; // Initialize _userSessionService
        }

        // GET: Transactions
        public IActionResult Index()
        {
            int userId = _userSessionService.GetLoggedInUserId();

            if (userId == -1)
            {
                // If no user is logged in, handle accordingly (e.g., redirect to login page)
                return RedirectToAction("Login", "User");
            }
            else
            {
                // User is logged in, fetch transactions related to the logged-in user
                var transactions = _context.Transactions
                    .Include(t => t.Account)
                    .Include(t => t.Category)
                    .Where(t => t.UserId == userId)  // Filter by UserId (assuming Account has a UserId field)
                    .ToList();

                // Return the filtered transactions to the view
                return View(transactions);
            }
        }
        
        // GET: Transactions/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Transactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PersonalFinanceApp.Models.Transaction transaction)
        {


            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(transaction);
            }

            var account = _context.Accounts.Find(transaction.AccountId);

            if (account == null)
            {
                ModelState.AddModelError("AccountId", "The selected account does not exist.");
                PopulateDropdowns();
                return View(transaction);
            }

            try
            {
                // Validate transaction type
                var validTypes = new[] { "Debit", "Credit", "Credit Card" };
                if (!validTypes.Contains(transaction.Type))
                {
                    ModelState.AddModelError("Type", "Invalid transaction type.");
                    PopulateDropdowns();
                    return View(transaction);
                }

                // Validate if debit exceeds balance
                if (transaction.Type == "Debit" && transaction.Amount > account.Balance)
                {
                    ModelState.AddModelError("Amount", "Transaction declined: Debit amount exceeds the account balance.");
                    PopulateDropdowns();
                    return View(transaction);
                }

                // Update account balance
                if (transaction.Type == "Debit" && account.Type != "Credit Card")
                {
                    account.Balance -= transaction.Amount;
                }
                else if (transaction.Type == "Credit" && account.Type != "Credit Card")
                {
                    account.Balance += transaction.Amount;
               
                }
                else if (transaction.Type == "Credit Card" && account.Type == "Credit Card")
                {
                    account.Balance -= transaction.Amount;
                    account.OutstandingBalance += transaction.Amount;
                }
                else if(account.Type=="Credit Card")
                {
                    ModelState.AddModelError("Account Type Mismatch", "Credit Card is the only valid transcation type for the chosen account.");
                    System.Diagnostics.Debug.WriteLine("Error Chech ------------------>", account.Type);
                    return View();
                }
                else
                {
                    ModelState.AddModelError("Account Type Mismatch", "Debit or Credit are the only valid transcation type for the chosen account.");
                    return View(transaction);
                }

                // Save changes synchronously
                int userId = _userSessionService.GetLoggedInUserId();
                if (userId == -1)
                {
                    return RedirectToAction("Login", "User");
                }
                if (account.UserId != userId)
                {
                    ModelState.AddModelError("User Account Mismatch", "Please login with the correct account.");
                    return RedirectToAction("Logout", "User");
                }
                transaction.UserId = userId;
                _context.Accounts.Update(account);
                _context.Transactions.Add(transaction);
                _context.SaveChanges(); // Synchronous call

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Unable to save changes. Please try again.");
                PopulateDropdowns();
                return View(transaction);
            }
        }

        // GET: Transactions/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var transaction = _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .FirstOrDefault(t => t.Id == id);

            if (transaction == null)
                return NotFound();

            PopulateDropdowns();
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PersonalFinanceApp.Models.Transaction transaction)
        {
            if (id != transaction.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var account = _context.Accounts.Find(transaction.AccountId);
                var originalTransaction = _context.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == id);

                if (account == null || originalTransaction == null)
                {
                    ModelState.AddModelError("AccountId", "The selected account does not exist.");
                    PopulateDropdowns();
                    return View(transaction);
                }

                // Reverse the effect of the original transaction
                if (originalTransaction.Type == "Debit" && account.Type != "Credit Card")
                {
                    account.Balance += originalTransaction.Amount;
                }
                else if (originalTransaction.Type == "Credit" && account.Type != "Credit Card")
                {
                    account.Balance -= originalTransaction.Amount;
                }
                else if (transaction.Type == "Credit Card" && account.Type== "Credit Card")
                {
                    account.Balance += transaction.Amount;
                    account.OutstandingBalance -= transaction.Amount;

                }

                // Apply the new transaction's impact
                if (transaction.Type == "Debit" && transaction.Amount > account.Balance)
                {
                    ModelState.AddModelError("Amount", "Transaction declined: Debit amount exceeds the account balance.");
                    PopulateDropdowns();
                    return View(transaction);
                }

                if (transaction.Type == "Debit" && account.Type != "Credit Card")
                {
                    account.Balance -= transaction.Amount;
                }
                else if (transaction.Type == "Credit" && account.Type != "Credit Card")
                {
                    account.Balance += transaction.Amount;
                }
                else if(transaction.Type =="Credit Card" && account.Type == "Credit Card")
                {
                    account.Balance -= transaction.Amount;
                    account.OutstandingBalance += transaction.Amount;
                }
                else
                {

                }
                int userId = _userSessionService.GetLoggedInUserId();
                if (userId == -1)
                {
                    return RedirectToAction("Login", "User");
                }
                if (account.UserId != userId)
                {

                }
                transaction.UserId = userId;

                _context.Accounts.Update(account);
                _context.Transactions.Update(transaction);
                _context.SaveChanges(); // Synchronous call

                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns();
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var transaction = _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .FirstOrDefault(t => t.Id == id);

            if (transaction == null)
                return NotFound();

            return View(transaction);
        }

        // POST: Transactions/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var transaction = _context.Transactions.Find(id);
            if (transaction != null)
            {
                var account = _context.Accounts.Find(transaction.AccountId);

                if (account != null)
                {
                    // Reverse transaction's impact on balance
                    if (transaction.Type == "Debit")
                    {
                        account.Balance += transaction.Amount;
                    }
                    else if (transaction.Type == "Credit")
                    {
                        account.Balance -= transaction.Amount;
                    }
                    else if (transaction.Type == "Credit Card")
                    {
                        account.Balance += transaction.Amount;
                        account.OutstandingBalance -= transaction.Amount;

                    }

                    _context.Accounts.Update(account);
                }

                _context.Transactions.Remove(transaction);
                _context.SaveChanges(); // Synchronous call
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns()
        {
            // Get the logged-in user's ID
            int userId = _userSessionService.GetLoggedInUserId();

            if (userId == -1)
            {
                // If no user is logged in, redirect to the login page (or handle accordingly)
                return;
            }

            // Fetch only accounts that belong to the logged-in user
            ViewBag.Accounts = _context.Accounts
                .Where(a => a.UserId == userId)  // Filter accounts by the logged-in userId
                .ToList();

            // Fetch categories with their subcategories, filtering by ParentCategoryId
            var categories = _context.Categories
                .Include(c => c.SubCategories)
                .Where(c => c.ParentCategoryId == null)
                .ToList();

            ViewBag.Categories = categories;

            // Fetch projects associated with the logged-in user
            ViewBag.Projects = _context.Projects
                .Where(p => p.UserId == userId)  // Filter projects by the logged-in userId
                .ToList();
            
        }

    }
}
