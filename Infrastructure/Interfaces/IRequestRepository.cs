using AbyKhedma.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IRequestRepository
    {
        public Task<Request> Create(Request request);
        public void Delete(Request request);
        public void Update(Request request);
        public IEnumerable<Request> GetAll();
        public Request GetById(int Id);
    }
}
