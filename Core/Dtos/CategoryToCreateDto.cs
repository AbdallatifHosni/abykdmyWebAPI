using AbyKhedma.Entities;
using Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Core.Dtos
{
    public class CategoryToCreateDto
    {
        public string CategoryName { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; }
        public string UrlPublicId { get; set; }
        public int OpeningStatementId { get; set; }
        public int ClosingStatementId { get; set; }
        public bool? IsSystem { get; set; } = false;
        public List<SubCategoryToCreateDto>? SubCategories { get; set; }

    }
}
