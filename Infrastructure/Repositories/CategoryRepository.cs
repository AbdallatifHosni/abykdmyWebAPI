using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger _logger;
        public CategoryRepository(ILogger<Category> logger, AppDbContext appDbContext)
        {
            _appDbContext= appDbContext;
            _logger = logger;
        }
        public async Task<Category> Create(Category  category)
        {
            try
            {
                if (category != null)
                {
                    var obj = _appDbContext.Add<Category>(category);
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
        public void Delete(Category  category)
        {
            try
            {
                if (category != null)
                {
                    var obj = _appDbContext.Remove(category);
                    if (obj != null)
                    {
                        _appDbContext.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<Category> GetAll()
        {
            try
            {
                var obj = _appDbContext.Categories.ToList();
                if (obj != null) return obj;
                else return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Category GetById(int Id)
        {
            try
            {
                if (Id != null)
                {
                    var Obj = _appDbContext.Categories.Where(x => x.Id == Id).AsNoTracking().FirstOrDefault();
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
        public void Update(Category category)
        {
            try
            {
                if (category != null)
                {
                    var obj = _appDbContext.Update(category);
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
