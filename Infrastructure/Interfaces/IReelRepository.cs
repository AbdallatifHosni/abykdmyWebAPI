using AbyKhedma.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IReelRepository
    {
        public Task<Reel> Create(Reel reel);
        public void Delete(Reel reel);
        public void Update(Reel reel);
        public IEnumerable<Reel> GetAll();
        public Reel GetById(int Id);
    }
}
