using Microsoft.AspNetCore.Mvc;
using PersonalFinanceApp.Data;
using PersonalFinanceApp.Models;
using PersonalFinanceApp.Service;

namespace PersonalFinanceApp.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserSessionService _userSessionService;

        public ProjectController(ApplicationDbContext context, IUserSessionService userSessionService)
        {
            _context = context;
            _userSessionService = userSessionService; // Initialize _userSessionService
        }

        public IActionResult Index()
        {
            // Get the logged-in user's ID
            int userId = _userSessionService.GetLoggedInUserId();

            // Check if user is logged in
            if (userId == -1)
            {
                // If not logged in, redirect to login page
                return RedirectToAction("Login", "User");
            }

            // Fetch projects for the logged-in user
            var projects = _context.Projects
                .Where(p => p.UserId == userId)  // Assuming there's a UserId field in the Project model
                .ToList();

            // Return the filtered projects to the view
            return View(projects);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Project project)
        {
            int userId = _userSessionService.GetLoggedInUserId();
            if (ModelState.IsValid)
            {
                project.UserId = userId;
                _context.Projects.Add(project);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }
        public IActionResult Edit(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost]
        public IActionResult Edit(Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Projects.Update(project);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        public IActionResult Delete(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var project = _context.Projects.Find(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}
