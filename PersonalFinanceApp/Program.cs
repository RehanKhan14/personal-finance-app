using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Data;
using PersonalFinanceApp.Models;
using PersonalFinanceApp.Service;

var builder = WebApplication.CreateBuilder(args);

// Fetch the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register ApplicationDbContext with MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString)); // UseMySQL is from MySql.EntityFrameworkCore

// Enable session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Set session timeout
    options.Cookie.HttpOnly = true;                   // Security setting
    options.Cookie.IsEssential = true;                // Make the cookie essential
});

builder.Services.AddScoped<IUserSessionService, UserSessionService>();

// Add IHttpContextAccessor to use session
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Add services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed test data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate(); // Ensure the database is up to date

    // Seed Accounts
    if (!context.Accounts.Any())
    {
        context.Accounts.AddRange(
            new Account { Name = "Savings Account", Balance = 5000, Type = "Savings" },
            new Account { Name = "Current", Balance = 1000, Type = "Current" },
            new Account { Name = "Credit Card", Balance = 500, Type= "Credit Card"}
        );
    }

    // Seed Categories
    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new Category { Name = "Groceries" },
            new Category { Name = "Transport" },
            new Category { Name = "Entertainment" },
            new Category { Name = "Education" },
            new Category { Name = "Bill Payment" },
            new Category { Name = "Family Support" },
            new Category { Name = "Health" }
        );
    }

    // Seed Projects
    if (!context.Projects.Any())
    {
        context.Projects.AddRange(
            new Project { Name = "Vacation Fund", Budget = 5000, EndDate = DateTime.Now.AddMonths(6) },
            new Project { Name = "Home Renovation", Budget = 10000, EndDate = DateTime.Now.AddYears(1) }
        );
    }

    // Save changes after seeding
    context.SaveChanges();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
