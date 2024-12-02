using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Data;
using PersonalFinanceApp.Models;
using PersonalFinanceApp.Controllers;
using PersonalFinanceApp.Service;

namespace PersonalFinanceApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserSessionService _userSessionService;
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context, IUserSessionService userSessionService)
        {
            _context = context;
            _userSessionService = userSessionService; // Initialize _userSessionService
        }

        // List all accounts
        public IActionResult Index()
        {
            int userid = _userSessionService.GetLoggedInUserId();
            if(userid==-1)
            {

            }
            else
            {
                var accounts = _context.Accounts
                .Where(account => account.UserId == userid)
                .ToList();
                return View(accounts);
            }
           return View();
        }

        // Create an account
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Account account)
        {
            if (ModelState.IsValid)
            {
                int userid = _userSessionService.GetLoggedInUserId();
                if (userid == -1)
                { //eroor
                }
                else
                {
                    account.UserId = userid;
                    _context.Accounts.Add(account);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(account);
        }

        // Edit account details
        public IActionResult Edit(int id)
        {
            var account = _context.Accounts.Find(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        [HttpPost]
        public IActionResult Edit(Account account)
        {
            if (ModelState.IsValid)
            {
                var existingAccount = _context.Accounts.Find(account.Id);
                if (existingAccount == null)
                {
                    return NotFound();
                }

                // Update the account details (excluding balance changes here)
                existingAccount.Name = account.Name;
                existingAccount.Type = account.Type;
                existingAccount.Balance = account.Balance;

                _context.Accounts.Update(existingAccount);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // Delete an account
        public IActionResult Delete(int id)
        {
            var account = _context.Accounts.Find(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var account = _context.Accounts.Find(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
