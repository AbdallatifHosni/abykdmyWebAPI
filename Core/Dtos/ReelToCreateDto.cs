namespace Core.Dtos
{
    public class ReelToCreateDto
    {
        public int CategoryId { get; set; }
        public string Url { get; set; }
        public string UrlPublicId { get; set; }
        public int FileType { get; set; }/*1: image, 2 : video, 3: file */

    }
}
