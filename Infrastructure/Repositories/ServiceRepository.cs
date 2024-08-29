using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger _logger;
        public ServiceRepository(ILogger<Service> logger, AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }
        public async Task<Service> Create(Service service)
        {
            try
            {
                if (service != null)
                {
                    var obj = _appDbContext.Add<Service>(service);
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
        public void Delete(Service service)
        {
            try
            {
                if (service != null)
                {
                    var obj = _appDbContext.Remove(service);
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
        public IEnumerable<Service> GetAll()
        {
            try
            {
                var obj = _appDbContext.Services.ToList();
                if (obj != null) return obj;
                else return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Service GetById(int Id)
        {
            try
            {
                if (Id != null)
                {
                    var Obj = _appDbContext.Services.FirstOrDefault(x => x.Id == Id);
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
        public void Update(Service service)
        {
            try
            {
                if (service != null)
                {
                    var obj = _appDbContext.Update(service);
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
