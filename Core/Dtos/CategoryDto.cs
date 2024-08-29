using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Url { get; set; }

        public List<ReelModel>  Reels { get; set; }
        public int GetHashCode(CategoryDto obj)
        {
            return obj.CategoryId.GetHashCode();
        }
        public bool Equals(CategoryDto x, CategoryDto y)
        {
            return x.CategoryId.Equals(y.CategoryId);
        }
    }
}
