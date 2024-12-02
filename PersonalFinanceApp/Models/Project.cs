namespace PersonalFinanceApp.Models
{
    public class Project
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Budget { get; set; }
        //public ICollection<Transaction> Transactions { get; set; }
    }

}
