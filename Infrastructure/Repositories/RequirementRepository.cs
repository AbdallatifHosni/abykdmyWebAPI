using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repositories
{
    public class RequirementRepository : IRequirementRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger _logger;
        public RequirementRepository(ILogger<Requirement> logger, AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }
        public async Task<Requirement> Create(Requirement requirement)
        {
            try
            {
                if (requirement != null)
                {
                    var obj = _appDbContext.Add<Requirement>(requirement);
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
        public void Delete(Requirement  requirement)
        {
            try
            {
                if (requirement != null)
                {
                    var obj = _appDbContext.Remove(requirement);
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
        public IEnumerable<Requirement> GetAll()
        {
            try
            {
                var obj = _appDbContext.Requirements.ToList();
                if (obj != null) return obj;
                else return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Requirement GetById(int Id)
        {
            try
            {
                if (Id != null)
                {
                    var Obj = _appDbContext.Requirements.FirstOrDefault(x => x.Id == Id);
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
        public void Update(Requirement requirement)
        {
            try
            {
                if (requirement != null)
                {
                    var obj = _appDbContext.Update(requirement);
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
