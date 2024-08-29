using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AbyKhedma.Services;
using Core.Dtos;
using Core.Models;
using AbyKhedma.Pagination;
using AbyKhedma.Persistance;
using AbyKhedma.Entities;
using Microsoft.EntityFrameworkCore;
using AbyKhedma.Dtos;

namespace AbyKhedma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StatementController> _logger;
        private readonly IUriService _uriService;
        private readonly AppDbContext _appDbContext;
        public AuditLogController(AppDbContext  appDbContext, IMapper mapper, IConfiguration configuration, ILogger<StatementController> logger, IUriService uriService)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _uriService = uriService;
        }

        [HttpGet("activities")]
        public ActionResult<List<AuditLogModel>> GetStatementList([FromQuery] FilterDto filterDto)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var filteredList = _appDbContext.AuditLogs.Include(el=>el.User).OrderByDescending(e=>e.Id)
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize).ToList();
           
            var totalRecords = _appDbContext.AuditLogs.Count();
            var res = _mapper.Map<List<AuditLogModel>>(filteredList);
            return Ok(PaginationHelper.CreatePagedReponse<AuditLogModel>(res, validFilter, totalRecords, _uriService, route));
        }
    }
}