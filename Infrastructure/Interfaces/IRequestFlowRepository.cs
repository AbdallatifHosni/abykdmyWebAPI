using AbyKhedma.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IRequestFlowRepository
    {
        public Task<RequestFlow> Create(RequestFlow requestFlow);
        public void Delete(RequestFlow requestFlow);
        public void Update(RequestFlow requestFlow);
        public IEnumerable<RequestFlow> GetAll();
        public RequestFlow GetById(int Id);
    }
}
