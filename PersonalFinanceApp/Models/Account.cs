using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceApp.Models
{
    public class Account
    {

        public int UserId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string Type { get; set; } // Savings, Credit, etc.
        public decimal? CreditLimit { get; set; } // Optional for credit cards
        public decimal? OutstandingBalance { get; set; } // Optional for credit cards
    }

}
