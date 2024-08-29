using AbyKhedma.Entities;

namespace Infrastructure.Interfaces
{
    public interface IRequirementRepository
    {
        public Task<Requirement> Create(Requirement requirement);
        public void Delete(Requirement requirement);
        public void Update(Requirement requirement);
        public IEnumerable<Requirement> GetAll();
        public Requirement GetById(int Id);
    }
}
