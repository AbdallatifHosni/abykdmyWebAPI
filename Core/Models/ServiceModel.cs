using AbyKhedma.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ServiceModel
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public bool IsActive { get; set; }
        public int CategoryID { get; set; }
        public decimal Price { get; set; }
        public int OpeningStatementId { get; set; }
        public int ClosingStatementId { get; set; }
        public CategoryModel Category { get; set; }
    }
}
