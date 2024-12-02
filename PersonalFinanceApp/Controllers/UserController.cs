using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Data;
using PersonalFinanceApp.Models;
using PersonalFinanceApp.Models.ViewModels;

namespace PersonalFinanceApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check user credentials in the database
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Authentication success - Redirect to dashboard
                    TempData["SuccessMessage"] = "Login successful!";
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    var userId = HttpContext.Session.GetString("UserId");
                    if (!string.IsNullOrEmpty(userId))
                    {
                        TempData["SuccessMessage"] = "Login successful!";
                        System.Diagnostics.Debug.WriteLine(userId);
                        return RedirectToAction("Dashboard", "Home");
                    }
                    else
                    {
                        // If session is not created successfully
                        ModelState.AddModelError(string.Empty, "Session creation failed.");
                    }
                }
                
                

                // Authentication failed
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }
        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

        // GET: Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the username already exists
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "This username is already taken.");
                    return View(model);
                }

                // Create a new user
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password // Consider hashing passwords for production use
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registration successful!";
                return RedirectToAction("Login");
            }

            return View(model);
        }
    }
}
