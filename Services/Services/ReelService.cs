using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using AutoMapper;
using Core.Dtos;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime;

namespace AbyKhedma.Services
{
    public class ReelService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ReelService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public List<ReelModel> GetReelList(int pageNumber, int pageSize)
        {
            var reels = _dbContext.Reels.Include(el => el.Category).Where(r => r.CreatedDate >= DateTime.Now.AddDays(-1)).OrderByDescending(el=>el.Id)
               .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return _mapper.Map<List<ReelModel>>(reels);
        }
        public List<ReelModel> GetReelsByCategoryId(int categoryId)
        {
            var reels = _dbContext.Reels.Where(r => r.CategoryId == categoryId && r.CreatedDate >= DateTime.Today).ToList();
            return _mapper.Map<List<ReelModel>>(reels);
        }
        public ReelModel GetReelById(int id)
        {
            var reel = _dbContext.Reels.FirstOrDefault(r => r.Id == id && r.CreatedDate >= DateTime.Today);
            return _mapper.Map<ReelModel>(reel);
        }
        public int AddReel(ReelToCreateDto reel)
        {
            var reelToCreate = _mapper.Map<Reel>(reel);
            _dbContext.Reels.Add(reelToCreate);
            _dbContext.SaveChanges();
            return reelToCreate.Id;
        }
        public int DeleteReel(int id)
        {
            var dbReel = _dbContext.Reels.FirstOrDefault(el => el.Id == id);
            if (dbReel != null)
            {
                _dbContext.Reels.Remove(dbReel);
                _dbContext.SaveChanges();
                return 1;
            }
            return 0;
        }
        public int AddReelList(List<ReelToCreateDto> reels)
        {
            var reelsToCreate = _mapper.Map<List<Reel>>(reels);
            _dbContext.Reels.AddRange(reelsToCreate);
            _dbContext.SaveChanges();
            return 1;
        }
    }
}
