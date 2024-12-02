namespace PersonalFinanceApp.Models
{
    public class Transaction
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        //public string Description { get; set; }
        public int? CategoryId { get; set; } // Nullable
        //public int? ProjectId { get; set; } // Nullable foreign key
        public Category? Category { get; set; } // Navigation property
        public Account? Account { get; set; } // Navigation property
        //public Project? Project { get; set; } // Navigation property
    }
}
