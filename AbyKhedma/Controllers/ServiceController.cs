using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AbyKhedma.Services;
using Core.Dtos;
using Core.Models;
using AbyKhedma.Pagination;
using AbyKhedma.Entities;

namespace AbyKhedma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServiceController> _logger;
        private readonly ServiceService _serviceService;
        private readonly IUriService _uriService;
        public ServiceController(ServiceService  serviceService, IMapper mapper, IConfiguration configuration, ILogger<ServiceController> logger, IUriService uriService)
        {
            _serviceService = serviceService;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _uriService = uriService;
        }

        [HttpGet("getAll")]
        public ActionResult<ServiceModel> GetServiceList([FromQuery] FilterDto filterDto)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var services = _serviceService.GetServiceList();

            var filteredList = services
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = services.Count();
            return Ok(PaginationHelper.CreatePagedReponse<ServiceModel>(filteredList, validFilter, totalRecords, _uriService, route));
             
        }
        [HttpGet("get/{id}")]
        public ActionResult<ServiceModel> GetServiceList(int id)
        {
            var service = _serviceService.GetServiceById(id);
            return Ok(new { Succeeded = true, Data = service, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("getByCategory/{categoryId}")]
        public ActionResult<List<ServiceModel>> GetServiceListByCategoryId([FromQuery] FilterDto filterDto,int categoryId)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var services = _serviceService.GetServiceByCategory(categoryId);

            var filteredList = services
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = services.Count();
            return Ok(PaginationHelper.CreatePagedReponse<ServiceModel>(filteredList, validFilter, totalRecords, _uriService, route));
        }
        [HttpPost("add")]
        public ActionResult<Task> AddService(ServiceToCreateDto  serviceToCreateDto)
        {
            var serviceId = _serviceService.AddService(serviceToCreateDto);
            if (serviceId == 0)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Failed to save", Errors = new string[] { } });
            }
            return Ok(new { Succeeded = true, Data = new { Id = serviceId }, Message = string.Empty, Errors = new string[] { } });
        }
    }
}