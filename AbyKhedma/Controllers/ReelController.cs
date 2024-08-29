using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AbyKhedma.Services;
using Core.Dtos;
using Core.Models;
using AbyKhedma.Pagination;
using AbyKhedma.Entities;
using Core.Common;

namespace AbyKhedma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReelController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReelController> _logger;
        private readonly ReelService _reelService;
        private readonly IUriService _uriService;
        public ReelController(ReelService  reelService, IMapper mapper, IConfiguration configuration, ILogger<ReelController> logger, IUriService uriService)
        {
            _reelService = reelService;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
                _uriService = uriService;
        }

        [HttpGet("getAll")]
        public ActionResult<ReelModel> GetReels([FromQuery] FilterDto filterDto)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var reels = _reelService.GetReelList(filterDto.PageNumber, filterDto.PageSize);

            var filteredList = reels
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = reels.Count();
            return Ok(PaginationHelper.CreatePagedReponse<ReelModel>(filteredList, validFilter, totalRecords, _uriService, route));
        }
        [HttpGet("getCategoriesReels")]
        public ActionResult<List<CategoryDto>> GetCategoriesReels([FromQuery] FilterDto filterDto)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var reels = _reelService.GetReelList(filterDto.PageNumber, filterDto.PageSize);
            IEqualityComparer<CategoryDto> customComparer =
                   new PropertyComparer<CategoryDto>("CategoryId");
            

            var categories = reels.Select(el => el.Category).Select(el=>new CategoryDto { CategoryId=el.Id,  CategoryName=el.CategoryName,  Url=el.Url}).Distinct(customComparer).ToList();
            foreach (var category in categories)
            {
                category.Reels=reels.Where(el=>el.CategoryId==category.CategoryId).ToList();
            }
            var totalRecords = categories.Count();

            return Ok(PaginationHelper.CreatePagedReponse<CategoryDto>(categories, validFilter, totalRecords, _uriService, route));
        }
        [HttpGet("get/{id}")]
        public ActionResult<ReelModel> GetReelById(int id)
        {
            var reelModel = _reelService.GetReelById(id);
            return Ok(new { Succeeded = true, Data = reelModel, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("getByCategoryId/{categoryId}")]
        public ActionResult<List<ReelModel>> GetOpeningReels([FromQuery] FilterDto filterDto,int categoryId)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var reelModels = _reelService.GetReelsByCategoryId(categoryId);

            var filteredList = reelModels
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = reelModels.Count();
            return Ok(PaginationHelper.CreatePagedReponse<ReelModel>(filteredList, validFilter, totalRecords, _uriService, route));
        }
         
        [HttpPost("add")]
        public ActionResult<Task> AddReel(ReelToCreateDto  reelToCreateDto)
        {
            var reelId = _reelService.AddReel(reelToCreateDto);
            if (reelId == 0)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Bad Request", Errors = new string[] { } });
            }
            return Ok(new { Succeeded = true, Data = new {   }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpDelete("delete/{id}")]
        public ActionResult<Task> DeleteReel(int id)
        {
            var reelId = _reelService.DeleteReel(id);
            if (reelId == 0)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Bad Request", Errors = new string[] { } });
            }
            return Ok(new { Succeeded = true, Data = new { }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpPost("addList")]
        public ActionResult<Task> AddReelList(List<ReelToCreateDto> reelToCreateDtoList)
        {
            var reelId = _reelService.AddReelList(reelToCreateDtoList);
            if (reelId == 0)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Bad Request", Errors = new string[] { } });
            }
            return Ok(new { Succeeded = true, Data = new { }, Message = string.Empty, Errors = new string[] { } });
        }
    }
}