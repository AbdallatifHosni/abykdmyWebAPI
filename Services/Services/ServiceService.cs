using AbyKhedma.Entities;
using AutoMapper;
using Core.Dtos;
using Core.Models;
using Infrastructure.Interfaces;

namespace AbyKhedma.Services
{
    public class ServiceService
    {
        private readonly IServiceRepository _dbContext;
        private readonly IMapper _mapper;

        public ServiceService(IServiceRepository dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public List<ServiceModel> GetServiceList()
        {
            var services = _dbContext.GetAll().Where(el => el.IsSystem != true).ToList();
            return _mapper.Map<List<ServiceModel>>(services);
        }
        public ServiceModel GetServiceById(int id)
        {
            var services = _dbContext.GetAll().Where(el => el.IsSystem != true).FirstOrDefault(s => s.Id == id);
            return _mapper.Map<ServiceModel>(services);
        }
        public List<ServiceModel> GetServiceByCategory(int categoryId)
        {
            var services = _dbContext.GetAll().Where(s => s.CategoryID == categoryId && s.IsSystem != true).ToList();
            if(services.Any())
            {
                services.ForEach(s => { 
                if(s.CreatedDate>s.UpdatedDate)
                    {
                        s.UpdatedDate = s.CreatedDate;
                    }
                });
            }
            return _mapper.Map<List<ServiceModel>>(services);
        }
        public int AddService(ServiceToCreateDto serviceToCreateDto)
        {
            var dbService = _dbContext.GetAll()
                            .Where(x => x.ServiceName == serviceToCreateDto.ServiceName && x.CategoryID == serviceToCreateDto.CategoryID).FirstOrDefault();
            if (dbService != null)
            {
                return dbService.Id;
            }
            var categoryToCreate = _mapper.Map<Service>(serviceToCreateDto);
            var createdCategory = _dbContext.Create(categoryToCreate);
            return createdCategory.Id;
        }
    }
}
