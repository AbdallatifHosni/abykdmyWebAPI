using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repositories
{
    public class RequestFlowRepository : IRequestFlowRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger _logger;
        public RequestFlowRepository(ILogger<RequestFlow> logger, AppDbContext appDbContext)
        {
            _appDbContext= appDbContext;
            _logger = logger;
        }
        public async Task<RequestFlow> Create(RequestFlow  requestFlow)
        {
            try
            {
                if (requestFlow != null)
                {
                    var obj = _appDbContext.Add<RequestFlow>(requestFlow);
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
        public void Delete(RequestFlow  requestFlow)
        {
            try
            {
                if (requestFlow != null)
                {
                    var obj = _appDbContext.Remove(requestFlow);
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
        public IEnumerable<RequestFlow> GetAll()
        {
            try
            {
                var obj = _appDbContext.RequestFlows.ToList();
                if (obj != null) return obj;
                else return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public RequestFlow GetById(int Id)
        {
            try
            {
                if (Id != null)
                {
                    var Obj = _appDbContext.RequestFlows.FirstOrDefault(x => x.Id == Id);
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
        public void Update(RequestFlow requestFlow)
        {
            try
            {
                if (requestFlow != null)
                {
                    var obj = _appDbContext.Update(requestFlow);
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
