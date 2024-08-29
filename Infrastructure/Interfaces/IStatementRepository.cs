using AbyKhedma.Entities;

namespace Infrastructure.Interfaces
{
    public interface IStatementRepository
    {
        public Task<Statement> Create(Statement statement);
        public void Delete(Statement statement);
        public void Update(Statement statement);
        public IEnumerable<Statement> GetAll();
        public Statement GetById(int Id);
    }
}
