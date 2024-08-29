using AbyKhedma.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<Category> Create(Category category);
        public void Delete(Category category);
        public void Update(Category category);
        public IEnumerable<Category> GetAll();
        public Category GetById(int Id);
    }
}
