using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repositories
{
    public class StatementRepository : IStatementRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger _logger;
        public StatementRepository(ILogger<Statement> logger, AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }
        public async Task<Statement> Create(Statement statement)
        {
            try
            {
                if (statement != null)
                {
                    var obj = _appDbContext.Add<Statement>(statement);
                    await _appDbContext.SaveChangesAsync();
                    return obj.Entity;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Delete(Statement statement)
        {
            try
            {
                if (statement != null)
                {
                    var obj = _appDbContext.Remove(statement);
                    if (obj != null)
                    {
                        _appDbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Statement> GetAll()
        {
            try
            {
                var obj = _appDbContext.Statements.ToList();
                if (obj != null) return obj;
                else return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Statement GetById(int Id)
        {
            try
            {
                if (Id != null)
                {
                    var Obj = _appDbContext.Statements.FirstOrDefault(x => x.Id == Id);
                    if (Obj != null) return Obj;
                    else return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Update(Statement statement)
        {
            try
            {
                if (statement != null)
                {
                    var obj = _appDbContext.Update(statement);
                    if (obj != null) _appDbContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
