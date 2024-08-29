using AbyKhedma.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> Create(User user);
        public void Delete(User user);
        public void Update(User user);
        public IEnumerable<User> GetAll();
        public User GetById(int Id);
    }
}
