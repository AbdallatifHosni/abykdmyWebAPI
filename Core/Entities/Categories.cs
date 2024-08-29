using System.ComponentModel.DataAnnotations;

namespace AbyKhedma.Entities
{
    public class Category : BaseEntity
    {
        [MaxLength(150)]
        public string CategoryName { get; set; }
        [MaxLength(250)]
        public string? Description { get; set; }
        public int? OpeningStatementId { get; set; }
        public int? ClosingStatementId { get; set; }
        [MaxLength(400)]
        public string? Url { get; set; }
        [MaxLength(30)]
        public string? UrlPublicId { get; set; }
        public int? ParentCategoryId { get; set; } = 0;
        public bool? HasChilds { get; set; } =false;
        public bool? IsSystem { get; set; }=false;
        //  public List<Service>?  Services { get; set; }


    }
}
