﻿namespace PersonalFinanceApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //public Project? Project { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; }
    }

}
