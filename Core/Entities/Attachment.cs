using System.ComponentModel.DataAnnotations;

namespace AbyKhedma.Entities
{
    public class Attachment
    {
        [Key]
        [MaxLength(30)]
        public string PublicId { get; set; }
        [MaxLength(200)]
        public string Url { get; set; }
        public int FileType { get; set; }/*1: image, 2 : video, 3: file */
    }
}
