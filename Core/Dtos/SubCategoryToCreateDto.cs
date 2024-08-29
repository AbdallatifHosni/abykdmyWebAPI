namespace Core.Dtos
{
    public class SubCategoryToCreateDto
    {
        public string CategoryName { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? UrlPublicId { get; set; }
        public int? OpeningStatementId { get; set; }
        public int? ClosingStatementId { get; set; }
        public bool? HasChilds { get; set; }
        public int? ParentCategoryId { get; set; }
        public List<SubCategoryToCreateDto>? SubCategories { get; set; }

    }

}
