using AbyKhedma.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class ReelModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Url { get; set; }
        public int FileType { get; set; }/*1: image, 2 : video, 3: file */
        public DateTime CreatedDate { get; set; }
        public CategoryModel Category { get; set; }
    }
}
