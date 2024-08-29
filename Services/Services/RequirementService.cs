using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using AutoMapper;
using Core.Dtos;
using Core.Models;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace AbyKhedma.Services
{
    public class RequirementService
    {

        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public RequirementService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public List<RequirementModel> GetRequirementList()
        {
            var requirements = _appDbContext.Requirements.Include(r => r.Answers).Where(s=>  s.IsSystem != true).ToList();
            return _mapper.Map<List<RequirementModel>>(requirements);
        }
        public RequirementModel GetRequirementById(int id)
        {
            var requirement = _appDbContext.Requirements.Include(r => r.Answers).FirstOrDefault(s => s.Id == id && s.IsSystem != true);
            return _mapper.Map<RequirementModel>(requirement);
        }
        public List<RequirementModel> GetRequirementsByServiceId(int serviceId)
        {
            var services = _appDbContext.Requirements.Include(r => r.Answers).Where(s =>  s.ServiceID == serviceId ).ToList();
            return _mapper.Map<List<RequirementModel>>(services);
        }
        public List<RequirementModel> GetSystemRequirementsWithServiceRequirementsByServiceId(int serviceId)
        {
            var services = _appDbContext.Requirements.Include(r => r.Answers).Where(s => s.ServiceID == serviceId|| s.IsSystem==true).ToList();
            return _mapper.Map<List<RequirementModel>>(services);
        }
        public int EditRequirement(RequirementToEditDto  requirementToEditDto)
        {
            var requirementFromDB=_appDbContext.Requirements.FirstOrDefault(el=>el.Id == requirementToEditDto.Id);
            if (requirementFromDB != null)
            {
                var requirementToUpdate = _mapper.Map(requirementToEditDto,requirementFromDB);
                _appDbContext.Requirements.Update(requirementToUpdate);
                _appDbContext.SaveChanges();
                return requirementToUpdate.Id;
            }
            return 0;
        }
        public int AddRequirement(RequirementToCreateDto requirementToCreateDto)
        {
            var requirementToCreate = _mapper.Map<Requirement>(requirementToCreateDto);
            _appDbContext.Requirements.Add(requirementToCreate);
            _appDbContext.SaveChanges();
            return requirementToCreate.Id;
        }
        public int DeleteRequirement(int id)
        {
            var requirement = _appDbContext.Requirements.Include(r => r.Answers).FirstOrDefault(s => s.Id == id);
            if(requirement !=null && requirement.IsSystem==true) { return -1; }
            _appDbContext.Requirements.Remove(requirement);
            _appDbContext.SaveChanges();
            return 1;
        }
    }
}
