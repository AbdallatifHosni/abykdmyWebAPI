using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AbyKhedma.Services;
using Core.Dtos;
using Core.Models;
using AbyKhedma.Pagination;

namespace AbyKhedma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatementController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StatementController> _logger;
        private readonly IUriService _uriService;
        private readonly StatementService _statmenetService;
        public StatementController(StatementService  statmenetService, IMapper mapper, IConfiguration configuration, ILogger<StatementController> logger, IUriService uriService)
        {
            _statmenetService = statmenetService;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _uriService = uriService;
        }

        [HttpGet("getAll")]
        public ActionResult<List<StatementModel>> GetStatementList([FromQuery] FilterDto filterDto)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var statements = _statmenetService.GetStatementList();

            var filteredList = statements
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = statements.Count();
            return Ok(PaginationHelper.CreatePagedReponse<StatementModel>(filteredList, validFilter, totalRecords, _uriService, route));
        }
        [HttpGet("get/{id}")]
        public ActionResult<StatementModel> GetStatementById(int id)
        {
            var statementModel = _statmenetService.GetStatementById(id);
            return Ok(new { Succeeded = true, Data = statementModel, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("openingStatements")]
        public ActionResult<List<StatementModel>> GetOpeningStatements([FromQuery] FilterDto filterDto)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var statementModels = _statmenetService.GetOpeningStatements();

            var filteredList = statementModels
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = statementModels.Count();
            return Ok(PaginationHelper.CreatePagedReponse<StatementModel>(filteredList, validFilter, totalRecords, _uriService, route));
        }

        [HttpGet("closingStatements")]
        public ActionResult<List<StatementModel>> GetClosingStatements([FromQuery] FilterDto filterDto)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var statementModels = _statmenetService.GetClosingStatements();

            var filteredList = statementModels
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = statementModels.Count();
            return Ok(PaginationHelper.CreatePagedReponse<StatementModel>(filteredList, validFilter, totalRecords, _uriService, route));
        }

        [HttpPost("add")]
        public ActionResult<Task> AddStatement(StatementToCreateDto  statementToCreateDto)
        {
            var statementId = _statmenetService.AddStatement(statementToCreateDto);
            if (statementId == 0)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Failed to save", Errors = new string[] { } });
            }
            return Ok(new { Succeeded = true, Data = new { Id = statementId }, Message = string.Empty, Errors = new string[] { } });
        }
    }
}