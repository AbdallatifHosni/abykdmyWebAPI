using AbyKhedma.Entities;

namespace Infrastructure.Interfaces
{
    public interface IServiceRepository
    {
        public Task<Service> Create(Service service);
        public void Delete(Service service);
        public void Update(Service service);
        public IEnumerable<Service> GetAll();
        public Service GetById(int Id);
    }
}
