using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repositories
{
    public class ReelRepository : IReelRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger _logger;
        public ReelRepository(ILogger<Reel> logger, AppDbContext appDbContext)
        {
            _appDbContext= appDbContext;
            _logger = logger;
        }
        public async Task<Reel> Create(Reel  reel)
        {
            try
            {
                if (reel != null)
                {
                    var obj = _appDbContext.Add<Reel>(reel);
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
        public void Delete(Reel  reel)
        {
            try
            {
                if (reel != null)
                {
                    var obj = _appDbContext.Remove(reel);
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
        public IEnumerable<Reel> GetAll()
        {
            try
            {
                var obj = _appDbContext.Reels.ToList();
                if (obj != null) return obj;
                else return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Reel GetById(int Id)
        {
            try
            {
                if (Id != null)
                {
                    var Obj = _appDbContext.Reels.FirstOrDefault(x => x.Id == Id);
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
        public void Update(Reel reel)
        {
            try
            {
                if (reel != null)
                {
                    var obj = _appDbContext.Update(reel);
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
