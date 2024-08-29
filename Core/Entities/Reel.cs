using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbyKhedma.Entities
{
    public class Reel : BaseEntity
    {
        public int CategoryId { get; set; }
        [MaxLength(400)]
        public string Url { get; set; }
        [MaxLength(30)]
        public string UrlPublicId { get; set; }
        public int FileType { get; set; }/*1: image, 2 : video, 3: file */
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
