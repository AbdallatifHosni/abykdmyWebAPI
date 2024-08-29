using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
 

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger _logger;
        public UserRepository(ILogger<User> logger, AppDbContext appDbContext)
        {
            _appDbContext= appDbContext;
            _logger = logger;
        }
        public async Task<User> Create(User User)
        {
            try
            {
                if (User != null)
                {
                    var obj = _appDbContext.Add<User>(User);
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
        public void Delete(User user)
        {
            try
            {
                if (user != null)
                {
                    var obj = _appDbContext.Remove(user);
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
        public  IEnumerable<User> GetAll()
        {
            try
            {
                var obj =  _appDbContext.Users.ToList();
                if (obj != null) return obj;
                else return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public User GetById(int Id)
        {
            try
            {
                if (Id != null)
                {
                    var Obj = _appDbContext.Users.FirstOrDefault(x => x.Id == Id);
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
        public void Update(User User)
        {
            try
            {
                if (User != null)
                {
                    var obj = _appDbContext.Update(User);
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
