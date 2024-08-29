using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger _logger;
        public RequestRepository(ILogger<Request> logger, AppDbContext appDbContext)
        {
            _appDbContext= appDbContext;
            _logger = logger;
        }
        public async Task<Request> Create(Request  request)
        {
            try
            {
                if (request != null)
                {
                    var obj = _appDbContext.Add<Request>(request);
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
        public void Delete(Request  request)
        {
            try
            {
                if (request != null)
                {
                    var obj = _appDbContext.Remove(request);
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
        public IEnumerable<Request> GetAll()
        {
            try
            {
                var obj = _appDbContext.Requests.ToList();
                if (obj != null) return obj;
                else return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Request GetById(int Id)
        {
            try
            {
                if (Id != null)
                {
                    var Obj = _appDbContext.Requests.FirstOrDefault(x => x.Id == Id);
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
        public void Update(Request request)
        {
            try
            {
                if (request != null)
                {
                    var obj = _appDbContext.Update(request);
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
