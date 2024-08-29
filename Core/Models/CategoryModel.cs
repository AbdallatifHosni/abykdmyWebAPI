using AbyKhedma.Entities;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? UrlPublicId { get; set; }
        public StatementModel? OpeningStatement { get; set; }
        public StatementModel? ClosingStatement { get; set; }
        public int? OpeningStatementId { get; set; }
        public int? ClosingStatementId { get; set; }
        public int? ParentCategoryId { get; set; } = 0;
        public bool? HasChilds { get; set; } = false;
        public List<CategoryModel>? SubCategories { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
 
}
