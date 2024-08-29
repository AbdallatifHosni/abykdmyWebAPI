using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AbyKhedma.Services;
using Core.Dtos;
using Core.Models;
using AbyKhedma.Pagination;
using AbyKhedma.Persistance;
using AbyKhedma.Entities;
using AbyKhedma.Core.Common;
using Microsoft.EntityFrameworkCore;

namespace AbyKhedma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequirementController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RequirementController> _logger;
        private readonly RequirementService _requirementService;
        private readonly AppDbContext _appDbContext;
        private readonly IUriService _uriService;
        public RequirementController(RequirementService requirementService, AppDbContext  appDbContext, IMapper mapper, IConfiguration configuration, ILogger<RequirementController> logger, IUriService uriService)
        {
            _requirementService = requirementService;
            _appDbContext = appDbContext;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _uriService = uriService;
        }

        [HttpGet("getAll")]
        public ActionResult<RequirementModel> GetRequirementList([FromQuery] FilterDto filterDto)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var requirements = _requirementService.GetRequirementList();

            var filteredList = requirements
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = requirements.Count();

            return Ok(PaginationHelper.CreatePagedReponse<RequirementModel>(filteredList, validFilter, totalRecords, _uriService, route));
        }

        [HttpGet("get/{id}")]
        public ActionResult<RequirementModel> GetRequirementById(int id)
        {
            var requirementModel = _requirementService.GetRequirementById(id);
            return Ok(new { Succeeded = true, Data = requirementModel, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("getByService/{serviceId}")]
        public ActionResult<List<RequirementModel>> GetRequiremenListByServiceId([FromQuery] FilterDto filterDto, int serviceId)
        {
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var requirementModels = _requirementService.GetRequirementsByServiceId(serviceId);
            if (requirementModels != null && requirementModels.Count > 0)
            {

                requirementModels.ForEach(r =>
                {
                    if (r.Answers != null && r.Answers.Count > 0)
                    {
                        r.Answers.ForEach(answer =>
                        {
                            answer.RequirementId = r.Id;
                        });
                    }
                });
            }

            var filteredList = requirementModels
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = requirementModels.Count();

            return Ok(PaginationHelper.CreatePagedReponse<RequirementModel>(filteredList, validFilter, totalRecords, _uriService, route));
        }
        [HttpPost("add")]
        public ActionResult<Task> AddRequirement(RequirementToCreateDto requirementToCreateDto)
        {
            var requirementId = _requirementService.AddRequirement(requirementToCreateDto);
            if (requirementId == 0)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Failed to save", Errors = new string[] { } });
            }
             
            return Ok(new { Succeeded = true, Data = new { id = requirementId }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpPost("addAnswerToRequirement")]
        public async Task<ActionResult<Task>> AddAnswerToRequirement(AnswerToAddDto  answerToAddDto)
        {
            var answerToAdd=_mapper.Map<Answer>(answerToAddDto);
            await  _appDbContext.Answers.AddAsync(answerToAdd);
            await _appDbContext.SaveChangesAsync();
            if (answerToAdd ==null )
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Failed to save", Errors = new string[] { } });
            }

            return Ok(new { Succeeded = true, Data = new { id = answerToAdd.Id }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpPost("addRequirementToAnswer")]
        public async Task<ActionResult<Task>> AddRequirementToAnswer(SubAnswerRequirementToCreateDto  subAnswerRequirement)
        {

            var answerFromDb= await _appDbContext.Answers.FirstOrDefaultAsync(el => el.Id == subAnswerRequirement.AnswerId);
            if (answerFromDb == null)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Invalid answer data", Errors = new string[] { } });
            }
            var subRequirementToCreate = _mapper.Map<SubAnswerRequirement>(subAnswerRequirement);
            subRequirementToCreate.AnswerId = answerFromDb.Id;
            await _appDbContext.SubAnswerRequirements.AddAsync(subRequirementToCreate);
            await _appDbContext.SaveChangesAsync();
            answerFromDb.HasSubAnswerRequirement = 1;
              _appDbContext.Answers.Update(answerFromDb);
            await _appDbContext.SaveChangesAsync();
            return Ok(new { Succeeded = true, Data = new { id = subRequirementToCreate.Id }, Message = string.Empty, Errors = new string[] { } });
        }
        

        [HttpDelete("deleteAnswer")]
        public async Task<ActionResult<Task>> DeleteAnswerToRequirement(int id)
        {
          var answerToDelete=  await _appDbContext.Answers.FirstOrDefaultAsync(a=>a.Id==id);
            if (answerToDelete==null)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Invalid request data", Errors = new string[] { } });
            }
             
            _appDbContext.Answers.Remove(answerToDelete);
            await _appDbContext.SaveChangesAsync();

            return Ok(new { Succeeded = true, Data = new {   }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpPost("editRequirement")]
        public ActionResult<Task> EditRequirement(RequirementToEditDto  requirementToEditDto)
        {
            var requirementId = _requirementService.EditRequirement(requirementToEditDto);
            if (requirementId == 0)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Invalid request data", Errors = new string[] { } });
            }

            return Ok(new { Succeeded = true, Data = new { id = requirementId }, Message = string.Empty, Errors = new string[] { } });
        }

        [HttpDelete("delete/{id}")]
        public ActionResult<Task> DeleteRequirement(int id)
        {
            var roleClaim = User.Claims.FirstOrDefault(el => el.Type == "_role");//.FindFirst(ClaimTypes.Role).Value;
            if (roleClaim == null || !Constants.GetSystemRoles().Contains(roleClaim.Value.ToString()))
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "You are not authorized to do that", Errors = new string[] { } });
            }
            var requirementId = _requirementService.DeleteRequirement(id);
            if (requirementId == -1)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Can't delete system requirements", Errors = new string[] { } });
            }

            if (requirementId == 0)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Failed to save", Errors = new string[] { } });
            }

            return Ok(new { Succeeded = true, Data = new { id = requirementId }, Message = string.Empty, Errors = new string[] { } });
        }
    }
}