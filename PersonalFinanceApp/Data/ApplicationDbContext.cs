using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; } 

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var transactionEntries = ChangeTracker.Entries<Transaction>();

            foreach (var entry in transactionEntries)
            {
                var transaction = entry.Entity;
                var account = await Accounts.FindAsync(transaction.AccountId);

                if (account != null)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            // Update balance when a new transaction is added
                            account.Balance += transaction.Type == "Debit" ? -transaction.Amount : transaction.Amount;
                            break;

                        case EntityState.Modified:
                            // Reverse the effect of the original transaction
                            var originalAmount = entry.OriginalValues.GetValue<decimal>("Amount");
                            var originalType = entry.OriginalValues.GetValue<string>("Type");

                            account.Balance -= originalType == "Debit" ? -originalAmount : originalAmount;

                            // Apply the effect of the updated transaction
                            account.Balance += transaction.Type == "Debit" ? -transaction.Amount : transaction.Amount;
                            break;

                        case EntityState.Deleted:
                            // Update balance when a transaction is deleted
                            account.Balance -= transaction.Type == "Debit" ? -transaction.Amount : transaction.Amount;
                            break;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
